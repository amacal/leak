namespace Leak.Core.Net
{
    public static class PeerMessageExtensions
    {
        public static PeerMessage Encrypt(this PeerMessage message, RC4 key)
        {
            return new PeerMessage(key.Encrypt(message.ToBytes()));
        }

        public static PeerMessage Decrypt(this PeerMessage message, RC4 key)
        {
            return new PeerMessage(key.Decrypt(message.ToBytes()));
        }
    }
}