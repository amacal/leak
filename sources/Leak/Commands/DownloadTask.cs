using Leak.Core.IO;

namespace Leak.Commands
{
    public class DownloadTask
    {
        public TorrentDirectory Destination { get; set; }

        public MetainfoFile Metainfo { get; set; }

        public MetainfoTracker[] Trackers { get; set; }
    }
}