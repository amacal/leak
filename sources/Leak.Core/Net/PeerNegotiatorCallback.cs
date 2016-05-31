namespace Leak.Core.Net
{
    public interface PeerNegotiatorCallback
    {
        void OnConnect(PeerConnection connection);

        void OnTerminate(PeerConnection connection);

        void OnHandshake(PeerConnection connection, PeerHandshake handshake);
    }
}