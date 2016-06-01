using Leak.Core.IO;

namespace Leak.Commands
{
    public class GetMetadataTask
    {
        public byte[] Hash { get; set; }

        public MetainfoTracker[] Trackers { get; set; }
    }
}