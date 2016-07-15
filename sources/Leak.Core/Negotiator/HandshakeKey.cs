using Leak.Core.Network;

namespace Leak.Core.Negotiator
{
    public class HandshakeKey
    {
        private readonly RC4 inner;

        private HandshakeKey(RC4 key)
        {
            inner = key;
        }

        public HandshakeKey(HandshakeKeyOwnership ownership, byte[] secret, HandshakeHash hash)
        {
            switch (ownership)
            {
                case HandshakeKeyOwnership.Initiator:
                    inner = new RC4(Bytes.Hash("keyA", secret, hash.ToBytes()), 1024);
                    break;

                case HandshakeKeyOwnership.Receiver:
                    inner = new RC4(Bytes.Hash("keyB", secret, hash.ToBytes()), 1024);
                    break;
            }
        }

        public byte[] Encrypt(byte[] data)
        {
            return inner.Encrypt(data);
        }

        public byte[] Decrypt(byte[] data)
        {
            return inner.Decrypt(data);
        }

        public NetworkIncomingMessage Decrypt(NetworkIncomingMessage message)
        {
            byte[] encrypted = message.ToBytes();
            byte[] decrypted = inner.Decrypt(encrypted);

            return new NetworkIncomingMessage(message, decrypted);
        }

        public HandshakeKey Clone()
        {
            return new HandshakeKey(inner.Clone());
        }

        public void Acknowledge(int bytes)
        {
            inner.Skip(bytes);
        }
    }
}