using Leak.Common;

namespace Leak.Negotiator
{
    public interface HandshakeNegotiatorPassiveContext : HandshakeNegotiatorContext
    {
        PeerHash Peer { get; }

        HandshakeOptions Options { get; }

        FileHashCollection Hashes { get; }

        void OnRejected(HandshakeRejection rejection);
    }
}