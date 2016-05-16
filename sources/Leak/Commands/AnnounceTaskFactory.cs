﻿using Leak.Core;
using Leak.Core.IO;
using Pargos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Leak.Commands
{
    public static class AnnounceTaskFactory
    {
        public static IEnumerable<AnnounceTask> Find(ArgumentCollection arguments)
        {
            List<MetainfoTracker> trackers = new List<MetainfoTracker>();

            for (int i = 0; i < arguments.Count("tracker"); i++)
            {
                string value = arguments.GetString("tracker", i);
                Uri uri = new Uri(value, UriKind.Absolute);

                trackers.Add(new MetainfoTracker(uri));
            }

            for (int i = 0; i < arguments.Count("torrent"); i++)
            {
                string file = arguments.GetString("torrent", i);
                byte[] data = File.ReadAllBytes(file);

                MetainfoFile metainfo = new MetainfoFile(data);
                byte[] hash = metainfo.Hash;

                yield return new AnnounceTask
                {
                    Hash = hash,
                    Trackers = metainfo.Trackers.ToArray()
                };
            }

            for (int i = 0; i < arguments.Count("hash"); i++)
            {
                string value = arguments.GetString("hash", i);
                byte[] hash = Bytes.Parse(value);

                yield return new AnnounceTask
                {
                    Hash = hash,
                    Trackers = trackers.ToArray()
                };
            }
        }
    }
}