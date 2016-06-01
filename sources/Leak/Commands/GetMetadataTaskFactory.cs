using Leak.Core;
using Leak.Core.IO;
using Pargos;
using System;
using System.Collections.Generic;

namespace Leak.Commands
{
    public static class GetMetadataTaskFactory
    {
        public static IEnumerable<GetMetadataTask> Find(ArgumentCollection arguments)
        {
            List<MetainfoTracker> trackers = new List<MetainfoTracker>();

            for (int i = 0; i < arguments.Count("tracker"); i++)
            {
                string value = arguments.GetString("tracker", i);
                Uri uri = new Uri(value, UriKind.Absolute);

                trackers.Add(new MetainfoTracker(uri));
            }

            for (int i = 0; i < arguments.Count("hash"); i++)
            {
                string value = arguments.GetString("hash", i);
                byte[] hash = Bytes.Parse(value);

                yield return new GetMetadataTask
                {
                    Hash = hash,
                    Trackers = trackers.ToArray()
                };
            }
        }
    }
}