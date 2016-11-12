using Leak.Core.Cando;
using Leak.Core.Collector.Callbacks;

namespace Leak.Core.Collector.Extensions
{
    public class MetadataInstaller : PeerCollectorExtension
    {
        private readonly PeerCollectorContext context;

        public MetadataInstaller(PeerCollectorContext context)
        {
            this.context = context;
        }

        public void Install(CandoConfiguration cando)
        {
            cando.Extensions.Metadata(with =>
            {
                with.Bus = cando.Bus;
                with.Callback = new PeerCollectorToMetadata(context);
            });
        }
    }
}