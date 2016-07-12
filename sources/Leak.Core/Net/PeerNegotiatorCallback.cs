using Leak.Core.Network;

namespace Leak.Core.Net
{
    public interface PeerNegotiatorCallback
    {
        void OnConnect(NetworkConnection connection);

        void OnTerminate(NetworkConnection connection);

        void OnHandshake(NetworkConnection connection, PeerHandshake handshake);
    }
}