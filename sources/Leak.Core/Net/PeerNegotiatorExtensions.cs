using System;

namespace Leak.Core.Net
{
    public static class PeerNegotiatorExtensions
    {
        public static void Receive(this PeerNegotiatorAware context, int size, Action<PeerMessage> callback)
        {
            Func<PeerMessage, bool> predicate = message =>
            {
                return message.Length >= size;
            };

            context.Receive(predicate, callback);
        }

        public static void Receive(this PeerNegotiatorAware context, Action<PeerMessage> callback)
        {
            Func<PeerMessage, bool> predicate = message =>
            {
                return true;
            };

            context.Receive(predicate, callback);
        }

        public static void Send(this PeerNegotiatorAware context, PeerMessageFactory data, RC4 key)
        {
            context.Send(new Encryptor(data, key));
        }

        public static void Continue(this PeerNegotiatorAware context, PeerHandshake handshake)
        {
            context.Continue(handshake, x => x, x => x, (buffer, count) => buffer.Remove(count));
        }

        private class Encryptor : PeerMessageFactory
        {
            private readonly PeerMessageFactory decrypted;
            private readonly RC4 key;

            public Encryptor(PeerMessageFactory decrypted, RC4 key)
            {
                this.decrypted = decrypted;
                this.key = key;
            }

            public override PeerMessage GetMessage()
            {
                return decrypted.GetMessage().Encrypt(key);
            }
        }
    }
}