using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Loop;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollector
    {
        private readonly ConnectionLoop loop;
        private readonly PeerCollectorStorage storage;
        private readonly PeerCollectorConfiguration configuration;

        public PeerCollector(Action<PeerCollectorConfiguration> configurer)
        {
            this.configuration = new PeerCollectorConfiguration
            {
                Callback = new PeerCollectorCallbackToNothing()
            };

            configurer.Invoke(configuration);

            this.storage = new PeerCollectorStorage(configuration);
            this.loop = new ConnectionLoop(with =>
            {
                with.Callback = new PeerCollectorToLoop(storage, configuration);
            });
        }

        public PeerCollectorView CreateView()
        {
            return new PeerCollectorView(storage);
        }

        public PeerListenerCallback CreateListenerCallback()
        {
            return new PeerCollectorToListener(loop, storage);
        }

        public PeerConnectorCallback CreateConnectorCallback()
        {
            return new PeerCollectorToConnector(loop, storage);
        }
    }
}