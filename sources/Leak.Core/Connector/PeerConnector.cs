using Leak.Core.Common;
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

        public void Start()
        {
            context.Timer.Start(OnTick);
        }

        public void ConnectTo(FileHash hash, PeerAddress peer)
        {
            context.Queue.Add(new PeerConnectorTaskConnect(context, hash, peer));
        }

        private void OnTick()
        {
            context.Queue.Process();
        }
    }
}