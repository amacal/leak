namespace Leak.Core.Negotiator
{
    public interface HandshakeNegotiatorPassiveContext : HandshakeNegotiatorContext
    {
        HandshakePeer Peer { get; }

        HandshakeOptions Options { get; }

        HandshakeHashCollection Hashes { get; }

        void OnRejected(HandshakeRejection rejection);
    }
}