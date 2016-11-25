using Leak.Core.Bencoding;
using Leak.Core.Common;
using System.Collections.Generic;
using System.IO;

namespace Leak.Core.Metadata
{
    public class MetainfoBuilder
    {
        private readonly string directory;
        private readonly List<MetainfoEntry> entries;

        public MetainfoBuilder(string directory)
        {
            this.directory = directory;
            this.entries = new List<MetainfoEntry>();
        }

        public void AddFile(string path)
        {
            string name = path.Substring(directory.Length + 1);
            long size = new FileInfo(path).Length;

            entries.Add(new MetainfoEntry(name, size));
        }

        public byte[] ToBytes()
        {
            BencodedValue entries = Build();
            byte[] bytes = Bencoder.Encode(entries);

            return bytes;
        }

        public Metainfo ToMetainfo()
        {
            BencodedValue entries = Build();
            byte[] bytes = Bencoder.Encode(entries);

            return MetainfoFactory.FromBytes(bytes);
        }

        public FileHash ToHash()
        {
            BencodedValue entries = Build();
            byte[] bytes = Bencoder.Encode(entries);

            Metainfo metainfo = MetainfoFactory.FromBytes(bytes);
            FileHash hash = metainfo.Hash;

            return hash;
        }

        private BencodedValue Build()
        {
            BencodedValue entries = new BencodedValue
            {
                Dictionary = new[]
                {
                    new BencodedEntry
                    {
                        Key = new BencodedValue
                        {
                            Text = new BencodedText("name")
                        },
                        Value = new BencodedValue
                        {
                            Text = new BencodedText(this.entries[0].Name[0])
                        }
                    },
                    new BencodedEntry
                    {
                        Key = new BencodedValue
                        {
                            Text = new BencodedText("length")
                        },
                        Value = new BencodedValue
                        {
                            Number = new BencodedNumber(this.entries[0].Size)
                        }
                    },
                    new BencodedEntry
                    {
                        Key = new BencodedValue
                        {
                            Text = new BencodedText("pieces")
                        },
                        Value = new BencodedValue
                        {
                            Data = new BencodedData(new byte[20 * ((this.entries[0].Size - 1) / 16384 + 1)])
                        }
                    },
                    new BencodedEntry
                    {
                        Key = new BencodedValue
                        {
                            Text = new BencodedText("piece length")
                        },
                        Value = new BencodedValue
                        {
                            Number = new BencodedNumber(16384)
                        }
                    }
                }
            };

            return entries;
        }
    }
}