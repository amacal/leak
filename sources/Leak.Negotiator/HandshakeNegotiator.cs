using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public interface HandshakeNegotiator
    {
        HandshakeNegotiatorHooks Hooks { get; }

        HandshakeNegotiatorDependencies Dependencies { get; }

        void Start(NetworkConnection connection, HandshakeNegotiatorActiveContext context);

        void Handle(NetworkConnection connection, HandshakeNegotiatorPassiveContext context);
    }
}