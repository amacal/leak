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
            string name = path.Substring(directory.Length);
            long size = new FileInfo(path).Length;

            entries.Add(new MetainfoEntry(name, size));
        }

        public byte[] ToBytes()
        {
            BencodedValue entries = Build();
            byte[] bytes = Bencoder.Encode(entries);

            return bytes;
        }

        public FileHash GetHash()
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
                            Text = new BencodedText("abc")
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
                            Number = new BencodedNumber(123)
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
                            Data = new BencodedData(new byte[20])
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