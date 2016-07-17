using System;
using System.Collections.Generic;
using System.IO;
using Leak.Core.Bencoding;

namespace Leak.Core.IO
{
    public static class MetainfoFileExtensions
    {
        public static void Include(this MetainfoFileConfiguration configuration, string directory)
        {
            configuration.Path = directory;
            configuration.Includes = new List<string>();

            foreach (string file in Directory.GetFiles(directory))
            {
                configuration.Includes.Add(file.Substring(directory.Length).TrimStart(Path.DirectorySeparatorChar));
            }
        }

        private static byte[] Hash(MetainfoFileConfiguration configuration)
        {
            List<byte[]> hashes = new List<byte[]>();
            Queue<string> includes = new Queue<string>(configuration.Includes);

            byte[] buffer = new byte[configuration.PieceLength];
            int offset = 0, read = 0;

            while (includes.Count > 0)
            {
                string include = includes.Dequeue();
                long total = 0;

                using (FileStream stream = File.OpenRead(Path.Combine(configuration.Path, include)))
                {
                    while (total < stream.Length)
                    {
                        while (read < buffer.Length)
                        {
                            if ((read = stream.Read(buffer, 0, buffer.Length)) == 0)
                                break;

                            offset += read;
                            total += read;
                        }

                        if (read == buffer.Length)
                        {
                            read = 0;
                            hashes.Add(Bytes.Hash(buffer));
                        }
                    }
                }
            }

            if (read > 0)
            {
                Array.Resize(ref buffer, read);
                hashes.Add(Bytes.Hash(buffer));
            }

            byte[] hash = new byte[20 * hashes.Count];

            for (int i = 0; i < hashes.Count; i++)
            {
                Array.Copy(hashes[i], 0, hash, i * 20, 20);
            }

            return hash;
        }

        public static byte[] ToBinary(this MetainfoFileConfiguration configuration)
        {
            BencodedEntry[] info = new BencodedEntry[]
            {
                new BencodedEntry
                {
                    Key = new BencodedValue { Text = new BencodedText("pieces") },
                    Value = new BencodedValue { Data = new BencodedData(Hash(configuration)) }
                }
            };

            BencodedValue root = new BencodedValue
            {
                Dictionary = new[]
                {
                    new BencodedEntry
                    {
                        Key = new BencodedValue { Text = new BencodedText("info") },
                        Value = new BencodedValue
                        {
                            Dictionary = info
                        }
                    }
                }
            };

            return Bencoder.Encode(root);
        }
    }
}