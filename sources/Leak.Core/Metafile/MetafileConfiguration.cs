using Leak.Core.Common;

namespace Leak.Core.Metafile
{
    public class MetafileConfiguration
    {
        public FileHash Hash { get; set; }

        public string Destination { get; set; }

        public MetafileCallback Callback { get; set; }
    }
}