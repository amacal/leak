using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Leak.Bencoding;
using Leak.Common;

namespace Leak.Meta.Info
{
    public class MetainfoBuilder
    {
        private readonly string directory;
        private readonly List<Entry> entries;

        public MetainfoBuilder(string directory)
        {
            this.directory = directory;
            this.entries = new List<Entry>();
        }

        public void AddFile(string path)
        {
            string name = path.Substring(directory.Length + 1);
            long size = new FileInfo(path).Length;

            entries.Add(new Entry
            {
                Name = name,
                Path = path,
                Size = size
            });
        }

        public byte[] ToBytes()
        {
            BencodedValue entries = Build();
            byte[] bytes = Bencoder.Encode(entries);

            return bytes;
        }

        public Metainfo ToMetainfo()
        {
            byte[] ignore;

            return ToMetainfo(out ignore);
        }

        public Metainfo ToMetainfo(out byte[] bytes)
        {
            BencodedValue entries = Build();
            bytes = Bencoder.Encode(entries);
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
                            Text = new BencodedText(this.entries[0].Name)
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
                            Data = new BencodedData(ComputePieceHashes())
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

        private class Entry
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public long Size { get; set; }
        }

        private byte[] ComputePieceHashes()
        {
            long total = this.entries.Sum(x => x.Size);
            int pieces = (int)((total - 1) / 16384 + 1);

            byte[] buffer = new byte[16384];
            byte[] hash = new byte[pieces * 20];
            byte[] calculated;

            int offset = 0;
            int piece = 0;
            int read;

            foreach (Entry entry in entries)
            {
                using (FileStream stream = File.OpenRead(entry.Path))
                {
                    do
                    {
                        read = stream.Read(buffer, offset, buffer.Length - offset);

                        if (read > 0)
                        {
                            offset += read;
                        }

                        if (offset == buffer.Length)
                        {
                            using (HashAlgorithm algorithm = SHA1.Create())
                            {
                                calculated = algorithm.ComputeHash(buffer, 0, offset);
                                Array.Copy(calculated, 0, hash, piece * 20, 20);

                                offset = 0;
                                piece++;
                            }
                        }
                    }
                    while (read > 0);
                }
            }

            if (offset > 0)
            {
                using (HashAlgorithm algorithm = SHA1.Create())
                {
                    calculated = algorithm.ComputeHash(buffer, 0, offset);
                    Array.Copy(calculated, 0, hash, piece * 20, 20);
                }
            }

            return hash;
        }
    }
}