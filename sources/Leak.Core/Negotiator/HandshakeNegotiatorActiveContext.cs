namespace Leak.Core.Negotiator
{
    public interface HandshakeNegotiatorActiveContext : HandshakeNegotiatorContext
    {
        HandshakePeer Peer { get; }

        HandshakeHash Hash { get; }

        HandshakeOptions Options { get; }
    }
}