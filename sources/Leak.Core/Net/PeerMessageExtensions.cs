using Leak.Core.Network;

namespace Leak.Core.Net
{
    public static class PeerMessageExtensions
    {
        public static NetworkOutgoingMessage Encrypt(this NetworkOutgoingMessage message, RC4 key)
        {
            return new NetworkOutgoingMessage(key.Encrypt(message.ToBytes()));
        }

        public static NetworkIncomingMessage Decrypt(this NetworkIncomingMessage message, RC4 key)
        {
            return new NetworkIncomingMessage(message, key.Decrypt(message.ToBytes()));
        }
    }
}