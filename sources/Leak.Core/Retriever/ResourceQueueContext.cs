using Leak.Core.Collector;
using Leak.Core.Metadata;
using Leak.Core.Repository;
using System;

namespace Leak.Core.Retriever
{
    public class ResourceQueueContext
    {
        public ResourceStorage Storage { get; set; }

        public PeerCollectorView Collector { get; set; }

        public ResourceRetrieverCallback Callback { get; set; }

        public ResourceRepositorySession Repository { get; set; }

        public MetainfoProperties Properties { get; set; }

        public ResourceQueueContext Configure(Action<ResourceQueueContext> configurer)
        {
            return configurer.Configure(with =>
            {
                with.Storage = Storage;
                with.Collector = Collector;
                with.Callback = Callback;
                with.Properties = Properties;
            });
        }
    }
}