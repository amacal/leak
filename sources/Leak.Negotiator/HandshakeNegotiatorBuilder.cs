using Leak.Networking;

namespace Leak.Peer.Negotiator
{
    public class HandshakeNegotiatorBuilder
    {
        private readonly HandshakeNegotiatorDependencies dependencies;

        public HandshakeNegotiatorBuilder()
        {
            dependencies = new HandshakeNegotiatorDependencies();
        }

        public HandshakeNegotiatorBuilder WithNetwork(NetworkPool network)
        {
            dependencies.Network = network;
            return this;
        }

        public HandshakeNegotiator Build()
        {
            return Build(new HandshakeNegotiatorHooks());
        }

        public HandshakeNegotiator Build(HandshakeNegotiatorHooks hooks)
        {
            return new HandshakeNegotiatorInstance(dependencies, hooks);
        }
    }
}