namespace Leak.Core.Net
{
    public interface PeerNegotiatorActiveContext
    {
        byte[] Hash { get; }

        PeerConnection Connection { get; }

        PeerHandshakeOptions Options { get; }

        void Continue(PeerHandshakePayload handshake, PeerConnection connection);

        void Terminate();
    }
}