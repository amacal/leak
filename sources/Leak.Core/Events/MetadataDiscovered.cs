using Leak.Core.Common;
using Leak.Core.Metadata;

namespace Leak.Core.Events
{
    public class MetadataDiscovered
    {
        public FileHash Hash;

        public Metainfo Metainfo;
    }
}