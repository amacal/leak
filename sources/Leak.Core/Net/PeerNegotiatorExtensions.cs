using System;

namespace Leak.Core.Net
{
    public static class PeerNegotiatorExtensions
    {
        public static void Receive(this PeerConnection connection, int size, Action<PeerMessage> callback)
        {
            Func<PeerMessage, bool> predicate = message =>
            {
                return message.Length >= size;
            };

            connection.Receive(predicate, callback);
        }

        public static void Receive(this PeerConnection connection, Action<PeerMessage> callback)
        {
            Func<PeerMessage, bool> predicate = message =>
            {
                return true;
            };

            connection.Receive(predicate, callback);
        }

        public static void Send(this PeerConnection connection, PeerMessageFactory data, RC4 key)
        {
            connection.Send(new Encryptor(data, key));
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