using Leak.Core.Common;
using Leak.Core.Metadata;

namespace Leak.Core.Omnibus
{
    public class OmnibusConfiguration
    {
        public Metainfo Metainfo { get; set; }

        public Bitfield Bitfield { get; set; }

        public OmnibusCallback Callback { get; set; }
    }
}