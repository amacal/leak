using Leak.Core.Client.Configuration;
using Leak.Core.Common;
using Leak.Core.Core;

namespace Leak.Core.Client
{
    public class PeerClientConfiguration
    {
        public PeerHash Peer { get; set; }

        public string Destination { get; set; }

        public string[] Countries { get; set; }

        public LeakBusCallback[] Subscribers { get; set; }

        public PeerClientCallback Callback { get; set; }

        public PeerClientListenerBuilder Listener { get; set; }

        public PeerClientConnectorBuilder Connector { get; set; }

        public PeerClientMetadataBuilder Metadata { get; set; }

        public PeerClientPeerExchangeBuilder PeerExchange { get; set; }

        public PeerClientDownloadBuilder Download { get; set; }
    }
}