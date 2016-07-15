using Leak.Core.Network;

namespace Leak.Core.Net
{
    public static class PeerMessageExtensions
    {
        public static NetworkOutgoingMessageBytes Encrypt(this NetworkOutgoingMessageBytes message, RC4 key)
        {
            return new NetworkOutgoingMessageBytes(key.Encrypt(message.ToBytes()));
        }

        public static NetworkIncomingMessage Decrypt(this NetworkIncomingMessage message, RC4 key)
        {
            return new NetworkIncomingMessage(message, key.Decrypt(message.ToBytes()));
        }
    }
}