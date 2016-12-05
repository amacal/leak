using Leak.Common;
using Leak.Core.Metadata;

namespace Leak.Core.Events
{
    public class MetafileVerified
    {
        public FileHash Hash;

        public Metainfo Metainfo;
    }
}