using Leak.Core.Core;

namespace Leak.Core.Cando.Metadata
{
    public class MetadataConfiguration
    {
        public LeakBus Bus { get; set; }

        public MetadataCallback Callback { get; set; }
    }
}