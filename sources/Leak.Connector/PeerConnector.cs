using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Connector
{
    public class PeerConnector
    {
        private readonly PeerConnectorContext context;

        public PeerConnector(PeerConnectorDependencies dependencies, PeerConnectorHooks hooks, PeerConnectorConfiguration configuration)
        {
            context = new PeerConnectorContext(dependencies, hooks, configuration);
        }

        public PeerConnectorDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public PeerConnectorConfiguration Configuration
        {
            get { return context.Configuration; }
        }

        public PeerConnectorHooks Hooks
        {
            get { return context.Hooks; }
        }

        public void Start()
        {
            context.Dependencies.Pipeline.Register(context.Queue);
        }

        public void ConnectTo(FileHash hash, NetworkAddress peer)
        {
            context.Queue.Add(new PeerConnectorTaskConnect(hash, peer));
        }
    }
}