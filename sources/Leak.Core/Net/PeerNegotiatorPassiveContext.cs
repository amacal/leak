namespace Leak.Core.Net
{
    public interface PeerNegotiatorPassiveContext
    {
        PeerConnection Connection { get; }

        PeerNegotiatorHashCollection Hashes { get; }

        void Continue(PeerHandshakePayload handshake, PeerConnection connection);

        void Terminate();
    }
}