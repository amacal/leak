using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Network;
using System;

namespace Leak.Core.Collector
{
    /// <summary>
    /// Represents a man-in-the-middle to mediate between low-level components
    /// and upper components in the stack. It provides an extra abstraction for
    /// looping each connection and offers additional benefits like connection
    /// filtering, congestion and peers management.
    /// </summary>
    public class PeerCollector
    {
        private readonly PeerCollectorContext context;

        public PeerCollector(Action<PeerCollectorConfiguration> configurer)
        {
            context = new PeerCollectorContext(configurer);
        }

        public PeerCollectorView CreateView(FileHash hash)
        {
            return new PeerCollectorView(hash, context);
        }

        public PeerListenerCallback CreateListenerCallback()
        {
            return new PeerCollectorListener(context);
        }

        public PeerConnectorCallback CreateConnectorCallback()
        {
            return new PeerCollectorConnector(context);
        }

        public NetworkPoolCallback CreatePoolCallback()
        {
            return new PeerCollectorPool(context);
        }
    }
}