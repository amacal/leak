using Leak.Core.IO;
using Pargos;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Leak.Commands
{
    public static class DownloadTaskFactory
    {
        public static IEnumerable<DownloadTask> Find(ArgumentCollection arguments)
        {
            for (int i = 0; i < arguments.Count("torrent"); i++)
            {
                string file = arguments.GetString("torrent", i);
                byte[] data = File.ReadAllBytes(file);

                MetainfoFile metainfo = new MetainfoFile(data);
                MetainfoTracker[] trackers = metainfo.Trackers.ToArray();

                string output = arguments.GetString("output");
                TorrentDirectory destination = new TorrentDirectory(metainfo, output);

                yield return new DownloadTask
                {
                    Destination = destination,
                    Metainfo = metainfo,
                    Trackers = trackers
                };
            }
        }
    }
}