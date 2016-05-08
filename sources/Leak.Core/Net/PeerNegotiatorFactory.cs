namespace Leak.Core.Net
{
    public static class PeerNegotiatorFactory
    {
        public static PeerNegotiator Create(PeerHandshake handshake)
        {
            return new EncryptedPeerNegotiator(handshake);
        }
    }
}