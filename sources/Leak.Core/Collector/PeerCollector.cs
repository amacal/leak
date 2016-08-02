using Leak.Core.Bouncer;
using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Loop;
using Leak.Core.Network;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollector
    {
        private readonly object synchronized;

        private readonly ConnectionLoop loop;
        private readonly PeerBouncer bouncer;
        private readonly PeerCollectorStorage storage;
        private readonly PeerCollectorConfiguration configuration;

        public PeerCollector(Action<PeerCollectorConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new PeerCollectorCallbackNothing();
            });

            synchronized = new object();
            storage = new PeerCollectorStorage(configuration);

            loop = new ConnectionLoop(with =>
            {
                with.Callback = new PeerCollectorLoop(storage, configuration, synchronized);
            });

            bouncer = new PeerBouncer(with =>
            {
                with.Callback = new PeerCollectorBouncer();
            });
        }

        public PeerCollectorView CreateView()
        {
            return new PeerCollectorView(storage);
        }

        public PeerListenerCallback CreateListenerCallback()
        {
            return new PeerCollectorListener(configuration.Callback, bouncer, loop, storage, synchronized);
        }

        public PeerConnectorCallback CreateConnectorCallback()
        {
            return new PeerCollectorConnector(configuration.Callback, bouncer, loop, storage, synchronized);
        }

        public NetworkPoolCallback CreatePoolCallback()
        {
            return new PeerCollectorPool(configuration.Callback, bouncer, storage, synchronized);
        }
    }
}