using Leak.Common;

namespace Leak.Core.Negotiator
{
    public class HandshakeKey
    {
        private readonly HandshakeRivestCipher inner;

        private HandshakeKey(HandshakeRivestCipher key)
        {
            inner = key;
        }

        public HandshakeKey(HandshakeKeyOwnership ownership, byte[] secret, FileHash hash)
        {
            switch (ownership)
            {
                case HandshakeKeyOwnership.Initiator:
                    inner = new HandshakeRivestCipher(Bytes.Hash("keyA", secret, hash.ToBytes()), 1024);
                    break;

                case HandshakeKeyOwnership.Receiver:
                    inner = new HandshakeRivestCipher(Bytes.Hash("keyB", secret, hash.ToBytes()), 1024);
                    break;
            }
        }

        public byte[] Encrypt(byte[] data)
        {
            return inner.Encrypt(data);
        }

        public byte[] Encrypt(byte[] data, int index, int count)
        {
            return inner.Encrypt(data, index, count);
        }

        public byte[] Decrypt(byte[] data)
        {
            return inner.Decrypt(data);
        }

        public void Decrypt(byte[] data, int index, int count)
        {
            inner.Decrypt(data, index, count);
        }

        public NetworkIncomingMessage Decrypt(NetworkIncomingMessage message)
        {
            byte[] encrypted = message.ToBytes();
            byte[] decrypted = inner.Decrypt(encrypted);

            return new HandshakeKeyMessage(message, decrypted);
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