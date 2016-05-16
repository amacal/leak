using Leak.Core.IO;

namespace Leak.Commands
{
    public class AnnounceTask
    {
        public byte[] Hash { get; set; }

        public MetainfoTracker[] Trackers { get; set; }
    }
}