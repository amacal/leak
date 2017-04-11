using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeNegotiatorInstance : HandshakeNegotiator
    {
        private readonly HandshakeNegotiatorDependencies dependencies;
        private readonly HandshakeNegotiatorHooks hooks;

        public HandshakeNegotiatorInstance(HandshakeNegotiatorDependencies dependencies, HandshakeNegotiatorHooks hooks)
        {
            this.dependencies = dependencies;
            this.hooks = hooks;
        }

        public HandshakeNegotiatorHooks Hooks
        {
            get { return hooks; }
        }

        public HandshakeNegotiatorDependencies Dependencies
        {
            get { return dependencies; }
        }

        public void Start(NetworkConnection connection, HandshakeNegotiatorActiveContext context)
        {
            new HandshakeNegotiatorActive(dependencies.Network, connection, context, hooks).Execute();
        }

        public void Handle(NetworkConnection connection, HandshakeNegotiatorPassiveContext context)
        {
            new HandshakeNegotiatorPassive(dependencies.Network, connection, context, hooks).Execute();
        }
    }
}