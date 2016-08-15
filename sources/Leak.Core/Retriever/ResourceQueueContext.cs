using Leak.Core.Collector;
using Leak.Core.Metadata;
using Leak.Core.Metamine;
using Leak.Core.Omnibus;
using Leak.Core.Repository;
using System;

namespace Leak.Core.Retriever
{
    public class ResourceQueueContext
    {
        public OmnibusBitfield Omnibus { get; set; }

        public MetamineBitfield Metamine { get; set; }

        public PeerCollectorView Collector { get; set; }

        public ResourceRetrieverCallback Callback { get; set; }

        public ResourceRepositorySession Repository { get; set; }

        public MetainfoProperties Properties { get; set; }

        public ResourceQueueContext Configure(Action<ResourceQueueContext> configurer)
        {
            return configurer.Configure(with =>
            {
                with.Omnibus = Omnibus;
                with.Metamine = Metamine;
                with.Collector = Collector;
                with.Callback = Callback;
                with.Properties = Properties;
            });
        }
    }
}