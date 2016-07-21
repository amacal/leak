using Leak.Core.Bencoding;
using Leak.Core.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Leak.Core.Metadata
{
    public static class MetainfoFactory
    {
        public static MetainfoFile FromFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            BencodedValue decoded = Bencoder.Decode(bytes);

            Metainfo metainfo = DecodeMetainfo(decoded);
            string[] trackers = FindTrackers(decoded);

            return new MetainfoFile(metainfo, trackers);
        }

        private static Metainfo DecodeMetainfo(BencodedValue value)
        {
            BencodedValue info = value.Find("info", x => x);
            FileHash hash = ComputeHash(info.Data);

            MetainfoEntry[] entries = FindEntries(info);
            MetainfoHash[] pieces = FindPieces(info);
            MetainfoProperties properties = FindProperties(info, entries, pieces);

            return new Metainfo(hash, entries, pieces, properties);
        }

        private static MetainfoProperties FindProperties(BencodedValue value, MetainfoEntry[] entries, MetainfoHash[] pieces)
        {
            long totalSize = entries.Sum(x => x.Size);
            int pieceSize = value.Find("piece length", x => (int)x.ToNumber());
            int blocks = (int)(pieces.Length * (totalSize / pieceSize) - totalSize % pieceSize / 32 / 1024 + 1);

            return new MetainfoProperties(totalSize, pieces.Length, pieceSize, blocks, 32 * 1024);
        }

        private static MetainfoHash[] FindPieces(BencodedValue value)
        {
            byte[] data = value.Find("pieces", x => x.Data.GetBytes());
            List<MetainfoHash> pieces = new List<MetainfoHash>();

            for (int i = 0; i < data.Length; i += 20)
            {
                pieces.Add(new MetainfoHash(Bytes.Copy(data, i, 20)));
            }

            return pieces.ToArray();
        }

        private static MetainfoEntry[] FindEntries(BencodedValue value)
        {
            List<MetainfoEntry> entries = new List<MetainfoEntry>();

            FindEntriesValue(value, entries);
            FindEntriesList(value, entries);

            return entries.ToArray();
        }

        private static void FindEntriesValue(BencodedValue value, List<MetainfoEntry> entries)
        {
            string name = value.Find("name", x => x.ToText());
            long size = value.Find("length", x => x.ToNumber());

            entries.Add(new MetainfoEntry(name, size));
        }

        private static void FindEntriesList(BencodedValue value, List<MetainfoEntry> entries)
        {
        }

        private static string[] FindTrackers(BencodedValue value)
        {
            HashSet<string> trackers = new HashSet<string>();

            FindTrackerValue(value, trackers);
            FindTrackerList(value, trackers);

            return trackers.ToArray();
        }

        private static void FindTrackerValue(BencodedValue value, HashSet<string> trackers)
        {
            value.Find("announce", node =>
            {
                if (node != null)
                {
                    trackers.Add(node.ToText());
                }

                return node;
            });
        }

        private static void FindTrackerList(BencodedValue value, HashSet<string> trackers)
        {
            value.Find("announce-list", node =>
            {
                if (node != null)
                {
                    foreach (string text in node.AllTexts())
                    {
                        if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
                        {
                            trackers.Add(text);
                        }
                    }
                }

                return node;
            });
        }

        private static FileHash ComputeHash(BencodedData data)
        {
            using (HashAlgorithm algorithm = SHA1.Create())
            {
                return new FileHash(algorithm.ComputeHash(data.GetBytes()));
            }
        }
    }
}