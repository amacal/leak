using F2F.Sandbox;
using Leak.Common;
using Leak.Core.Events;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using Leak.Core.Repository;
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
            Events = new EventsFixture(this);
        }

        public void Dispose()
        {
            Binary.Sandbox.Dispose();
        }

        private static BinaryFixture CreateDebianBinary()
        {
            FileSandbox sandbox = new FileSandbox(new EmptyFileLocator());
            List<BinaryPieceFixture> pieces = new List<BinaryPieceFixture>();
            List<string> files = new List<string>();

            byte[] data = Bytes.Random(16384 * 2 + 187);
            files.Add(sandbox.CreateFile("debian-8.5.0-amd64-CD-1.iso", data));

            for (int i = 0; i < data.Length; i += 16384)
            {
                int size = Math.Min(16384, data.Length - i);

                pieces.Add(new BinaryPieceFixture
                {
                    Index = i / 16384,
                    Size = size,
                    Blocks = new[]
                    {
                        new BinaryBlockFixture
                        {
                            Index = 0,
                            Size = size,
                            Data = new RepositoryBlockData(i / 16384, 0, new BinaryBlockData(data.Skip(i).Take(size).ToArray()))
                        }
                    }
                });
            }

            return new BinaryFixture
            {
                Sandbox = sandbox,
                Size = data.Length,
                Files = files.ToArray(),
                Pieces = pieces.ToArray(),
                Content = data
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
        public BinaryPieceFixture[] Pieces { get; set; }
        public byte[] Content { get; set; }
        public long Size { get; set; }
    }

    public class BinaryPieceFixture
    {
        public int Index { get; set; }
        public int Size { get; set; }
        public BinaryBlockFixture[] Blocks { get; set; }
    }

    public class BinaryBlockFixture
    {
        public int Index { get; set; }
        public int Size { get; set; }
        public RepositoryBlockData Data { get; set; }
    }

    public class BinaryBlockData : DataBlock
    {
        private readonly byte[] data;
        private readonly int offset;

        public BinaryBlockData(byte[] data)
        {
            this.data = data;
        }

        private BinaryBlockData(byte[] data, int offset)
        {
            this.data = data;
            this.offset = offset;
        }

        public int Size
        {
            get { return data.Length - offset; }
        }

        public byte this[int index]
        {
            get { return data[index + offset]; }
        }

        public void Write(DataBlockCallback callback)
        {
            callback.Invoke(data, offset, data.Length - offset);
        }

        public DataBlock Scope(int other)
        {
            return new BinaryBlockData(data, offset + other);
        }

        public void Dispose()
        {
        }
    }

    public class EventsFixture
    {
        public readonly MetadataMeasured MetadataMeasured;
        public readonly MetadataReceived MetadataReceived;

        public readonly BlockReceived[] BlockReceived;

        public EventsFixture(DataFixture owner)
        {
            MetadataMeasured = new MetadataMeasured
            {
                Hash = owner.Metadata.Hash,
                Peer = PeerHash.Random(),
                Size = owner.Metadata.Size
            };

            MetadataReceived = new MetadataReceived
            {
                Hash = owner.Metadata.Hash,
                Peer = PeerHash.Random(),
                Piece = owner.Metadata.Pieces[0].Index,
                Data = owner.Metadata.Pieces[0].Data
            };

            BlockReceived = new BlockReceived[owner.Binary.Pieces.Length];

            for (int i = 0; i < BlockReceived.Length; i++)
            {
                byte[] data = owner.Binary.Content.Skip(i * 16384).Take(16384).ToArray();

                BlockReceived[i] = new BlockReceived
                {
                    Hash = owner.Metadata.Hash,
                    Peer = PeerHash.Random(),
                    Piece = i,
                    Block = 0,
                    Payload = new BinaryBlockData(data)
                };
            }
        }
    }
}