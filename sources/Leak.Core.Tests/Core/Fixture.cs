using F2F.Sandbox;
using Leak.Core.Common;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Tests.Core
{
    public class Fixture : IDisposable
    {
        public readonly MetadataFixture Metadata;

        public Fixture()
        {
            Metadata = new MetadataFixture();
        }

        public void Dispose()
        {
            Metadata.Dispose();
        }

        public static void Sample(out FileHash hash, out byte[] metadata)
        {
            using (FileSandbox forBuilder = Sandbox.New())
            {
                MetainfoBuilder builder = new MetainfoBuilder(forBuilder.Directory);
                string file = forBuilder.CreateFile("abc.txt");

                builder.AddFile(file);

                hash = builder.ToHash();
                metadata = builder.ToBytes();
            }
        }

        public class MetadataFixture : IDisposable
        {
            public readonly HashFixture Debian;

            public MetadataFixture()
            {
                Debian = CreateDebian();
            }

            public void Dispose()
            {
            }

            private static HashFixture CreateDebian()
            {
                using (FileSandbox forBuilder = Sandbox.New())
                {
                    MetainfoBuilder builder = new MetainfoBuilder(forBuilder.Directory);
                    string file = forBuilder.CreateFile("debian-8.5.0-amd64-CD-1.iso", Bytes.Random(1024));

                    builder.AddFile(file);

                    byte[] content = builder.ToBytes();
                    HashFixture hash = new HashFixture
                    {
                        Hash = builder.ToHash(),
                        Pieces = Split(content),
                        Content = content,
                        Size = content.Length,
                    };

                    hash.Events = new EventsFixture(hash);
                    return hash;
                }
            }

            private static PieceFixture[] Split(byte[] data)
            {
                int size = 16384;
                List<PieceFixture> blocks = new List<PieceFixture>();

                for (int i = 0; i < data.Length; i += size)
                {
                    blocks.Add(new PieceFixture
                    {
                        Index = i / size,
                        Size = (data.Length - i / size) % size,
                        Data = data.Skip(i).Take(size).ToArray()
                    });
                }

                return blocks.ToArray();
            }

            public class HashFixture
            {
                public FileHash Hash { get; set; }
                public PieceFixture[] Pieces { get; set; }
                public byte[] Content { get; set; }
                public int Size { get; set; }
                public EventsFixture Events { get; set; }
            }

            public class PieceFixture
            {
                public int Index { get; set; }
                public int Size { get; set; }
                public byte[] Data { get; set; }
            }

            public class EventsFixture
            {
                public readonly MetadataMeasured MetadataMeasured;
                public readonly MetadataReceived MetadataReceived;

                public EventsFixture(HashFixture parent)
                {
                    MetadataMeasured = new MetadataMeasured
                    {
                        Hash = parent.Hash,
                        Peer = PeerHash.Random(),
                        Size = parent.Size
                    };

                    MetadataReceived = new MetadataReceived
                    {
                        Hash = parent.Hash,
                        Peer = PeerHash.Random(),
                        Piece = parent.Pieces[0].Index,
                        Data = parent.Pieces[0].Data
                    };
                }
            }
        }
    }
}