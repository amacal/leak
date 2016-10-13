using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Network;
using System;

namespace Leak.Core.Connector
{
    public class PeerConnectorContext
    {
        private readonly PeerConnectorConfiguration configuration;
        private readonly LeakQueue<PeerConnectorContext> queue;

        public PeerConnectorContext(Action<PeerConnectorConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new PeerConnectorCallbackNothing();
                with.Peer = new PeerHash(Bytes.Random(20));
                with.Pool = new NetworkPool();
            });

            queue = new LeakQueue<PeerConnectorContext>(this);
        }

        public PeerConnectorConfiguration Configuration
        {
            get { return configuration; }
        }

        public NetworkPool Pool
        {
            get { return configuration.Pool; }
        }

        public LeakQueue<PeerConnectorContext> Queue
        {
            get { return queue; }
        }
    }
}