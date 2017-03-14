using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Leak.Bencoding;
using Leak.Common;

namespace Leak.Meta.Info
{
    public static class MetainfoFactory
    {
        public static MetainfoFile FromFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            BencodedValue decoded = Bencoder.Decode(bytes);
            BencodedValue info = decoded.Find("info", x => x);

            Metainfo metainfo = DecodeMetainfo(info);
            string[] trackers = FindTrackers(decoded);

            return new MetainfoFile(metainfo, trackers);
        }

        public static Metainfo FromBytes(byte[] bytes)
        {
            BencodedValue decoded = Bencoder.Decode(bytes);
            Metainfo metainfo = DecodeMetainfo(decoded);

            return metainfo;
        }

        private static Metainfo DecodeMetainfo(BencodedValue value)
        {
            FileHash hash = ComputeHash(value.Data);

            MetainfoEntry[] entries = FindEntries(value);
            MetainfoHash[] pieces = FindPieces(value);
            MetainfoProperties properties = FindProperties(value, entries, pieces);

            return new Metainfo(hash, entries, pieces, properties);
        }

        private static MetainfoProperties FindProperties(BencodedValue value, MetainfoEntry[] entries, MetainfoHash[] pieces)
        {
            long totalSize = entries.Sum(x => x.Size);
            int blockSize = 16384;
            int pieceSize = value.Find("piece length", x => (int)x.ToInt64());

            return new MetainfoProperties(totalSize, pieces.Length, pieceSize, blockSize);
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
            string name = value.Find("name", x => x?.ToText(Encoding.UTF8));
            long? size = value.Find("length", x => x?.ToInt64());

            if (name != null && size != null)
            {
                entries.Add(new MetainfoEntry(name, size.Value));
            }
        }

        private static void FindEntriesList(BencodedValue value, List<MetainfoEntry> entries)
        {
            BencodedValue files = value.Find("files", x => x);

            if (files?.Array != null)
            {
                foreach (BencodedValue item in files.Array)
                {
                    long? size = item.Find("length", x => x?.ToInt64());
                    BencodedValue path = item.Find("path", x => x);

                    if (size != null && path?.Array != null)
                    {
                        List<string> names = new List<string>();

                        foreach (BencodedValue name in path.Array)
                        {
                            names.Add(name.ToText(Encoding.UTF8));
                        }

                        entries.Add(new MetainfoEntry(names.ToArray(), size.Value));
                    }
                }
            }
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