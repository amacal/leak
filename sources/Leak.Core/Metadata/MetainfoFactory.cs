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

            return new Metainfo(hash, entries);
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