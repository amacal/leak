using Leak.Core.Cando;
using Leak.Core.Collector.Extensions;
using System.Collections.Generic;

namespace Leak.Core.Collector
{
    public class PeerCollectorExtensionBuilder
    {
        private readonly PeerCollectorContext context;
        private readonly List<PeerCollectorExtension> items;

        public PeerCollectorExtensionBuilder(PeerCollectorContext context)
        {
            this.context = context;
            this.items = new List<PeerCollectorExtension>();
        }

        public void Include(PeerCollectorExtension extension)
        {
            items.Add(extension);
        }

        public void IncludeMetadata()
        {
            items.Add(new MetadataInstaller(context));
        }

        public void IncludePeerExchange()
        {
            items.Add(new PeerExchangeInstaller(context));
        }

        public void Apply(CandoConfiguration cando)
        {
            foreach (PeerCollectorExtension extension in items)
            {
                extension.Install(cando);
            }
        }
    }
}