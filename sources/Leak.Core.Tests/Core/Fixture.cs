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
        public readonly DataFixture Debian;

        public Fixture()
        {
            Debian = new DataFixture();
        }

        public void Dispose()
        {
            Debian.Dispose();
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
    }

    public class DataFixture : IDisposable
    {
        public readonly MetadataFixture Metadata;
        public readonly EventsFixture Events;
        public readonly BinaryFixture Binary;

        public DataFixture()
        {
            Binary = CreateDebianBinary();
            Metadata = CreateDebianMetadata(Binary);
            Events = new EventsFixture(Metadata);
        }

        public void Dispose()
        {
            Binary.Sandbox.Dispose();
        }

        private static BinaryFixture CreateDebianBinary()
        {
            FileSandbox sandbox = new FileSandbox(new EmptyFileLocator());
            List<string> files = new List<string>();

            byte[] data = Bytes.Random(16384 * 2 + 187);
            files.Add(sandbox.CreateFile("debian-8.5.0-amd64-CD-1.iso", data));

            return new BinaryFixture
            {
                Sandbox = sandbox,
                Size = data.Length,
                Files = files.ToArray(),
                Pieces = data.Length / 16384 + 1
            };
        }

        private static MetadataFixture CreateDebianMetadata(BinaryFixture binary)
        {
            MetainfoBuilder builder = new MetainfoBuilder(binary.Sandbox.Directory);

            foreach (string file in binary.Files)
            {
                builder.AddFile(file);
            }

            byte[] content = builder.ToBytes();
            MetadataFixture metadata = new MetadataFixture
            {
                Hash = builder.ToHash(),
                Metainfo = MetainfoFactory.FromBytes(content),
                Pieces = Split(content),
                Content = content,
                Size = content.Length,
            };

            return metadata;
        }

        private static MetadataPieceFixture[] Split(byte[] data)
        {
            int size = 16384;
            List<MetadataPieceFixture> blocks = new List<MetadataPieceFixture>();

            for (int i = 0; i < data.Length; i += size)
            {
                blocks.Add(new MetadataPieceFixture
                {
                    Index = i / size,
                    Size = (data.Length - i / size) % size,
                    Data = data.Skip(i).Take(size).ToArray()
                });
            }

            return blocks.ToArray();
        }
    }

    public class MetadataFixture
    {
        public FileHash Hash { get; set; }
        public Metainfo Metainfo { get; set; }
        public MetadataPieceFixture[] Pieces { get; set; }
        public byte[] Content { get; set; }
        public int Size { get; set; }
    }

    public class MetadataPieceFixture
    {
        public int Index { get; set; }
        public int Size { get; set; }
        public byte[] Data { get; set; }
    }

    public class BinaryFixture
    {
        public FileSandbox Sandbox { get; set; }
        public string[] Files { get; set; }
        public int Pieces { get; set; }
        public long Size { get; set; }
    }

    public class EventsFixture
    {
        public readonly MetadataMeasured MetadataMeasured;
        public readonly MetadataReceived MetadataReceived;

        public EventsFixture(MetadataFixture parent)
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