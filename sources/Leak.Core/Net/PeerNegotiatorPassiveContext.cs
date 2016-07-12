using Leak.Core.Network;

namespace Leak.Core.Net
{
    public interface PeerNegotiatorPassiveContext
    {
        NetworkConnection Connection { get; }

        PeerHandshakeOptions Options { get; }

        PeerNegotiatorHashCollection Hashes { get; }

        void Continue(PeerHandshakePayload handshake, NetworkConnection connection);

        void Terminate();
    }
}