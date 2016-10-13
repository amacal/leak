using Leak.Core.Common;
using Leak.Core.Core;
using System;

namespace Leak.Core.Connector
{
    public class PeerConnector
    {
        private readonly PeerConnectorContext context;

        public PeerConnector(Action<PeerConnectorConfiguration> configurer)
        {
            context = new PeerConnectorContext(configurer);
        }

        public void Start(LeakPipeline pipeline)
        {
            pipeline.Register(context.Queue);
        }

        public void ConnectTo(FileHash hash, PeerAddress peer)
        {
            context.Queue.Add(new PeerConnectorTaskConnect(hash, peer));
        }
    }
}