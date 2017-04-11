using Leak.Common;

namespace Leak.Peer.Negotiator
{
    public interface HandshakeNegotiatorActiveContext : HandshakeNegotiatorContext
    {
        PeerHash Peer { get; }

        FileHash Hash { get; }

        HandshakeOptions Options { get; }
    }
}