using Leak.Common;

namespace Leak.Negotiator
{
    public interface HandshakeNegotiatorActiveContext : HandshakeNegotiatorContext
    {
        PeerHash Peer { get; }

        FileHash Hash { get; }

        HandshakeOptions Options { get; }
    }
}