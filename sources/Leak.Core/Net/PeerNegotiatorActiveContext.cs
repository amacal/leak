using Leak.Core.Network;

namespace Leak.Core.Net
{
    public interface PeerNegotiatorActiveContext
    {
        byte[] Hash { get; }

        NetworkConnection Connection { get; }

        PeerHandshakeOptions Options { get; }

        void Continue(PeerHandshakePayload handshake, NetworkConnection connection);

        void Terminate();
    }
}