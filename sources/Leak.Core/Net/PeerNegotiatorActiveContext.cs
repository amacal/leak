namespace Leak.Core.Net
{
    public interface PeerNegotiatorActiveContext
    {
        byte[] Hash { get; }

        PeerConnection Connection { get; }

        void Continue(PeerHandshakePayload handshake, PeerConnection connection);

        void Terminate();
    }
}