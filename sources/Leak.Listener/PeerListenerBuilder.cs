using Leak.Common;
using Leak.Networking;

namespace Leak.Listener
{
    public class PeerListenerBuilder
    {
        private readonly PeerListenerDependencies dependencies;
        private readonly PeerListenerConfiguration configuration;

        public PeerListenerBuilder()
        {
            dependencies = new PeerListenerDependencies();
            configuration = new PeerListenerConfiguration();
        }

        public PeerListenerBuilder WithNetwork(NetworkPool network)
        {
            dependencies.Network = network;
            return this;
        }

        public PeerListenerBuilder WithPort(int port)
        {
            configuration.Port = new PeerListenerPortValue(port);
            return this;
        }

        public PeerListener Build()
        {
            return Build(new PeerListenerHooks());
        }

        public PeerListener Build(PeerListenerHooks hooks)
        {
            return new PeerListener(dependencies, hooks, configuration);
        }
    }
}