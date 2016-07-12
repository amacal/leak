using Leak.Core.Network;
using System;

namespace Leak.Core.Net
{
    public static class PeerNegotiatorExtensions
    {
        public static void Receive(this NetworkConnection connection, int size, Action<NetworkIncomingMessage> callback)
        {
            Func<NetworkIncomingMessage, bool> predicate = message =>
            {
                return message.Length >= size;
            };

            connection.Receive(predicate, callback);
        }

        public static void Receive(this NetworkConnection connection, Action<NetworkIncomingMessage> callback)
        {
            Func<NetworkIncomingMessage, bool> predicate = message =>
            {
                return true;
            };

            connection.Receive(predicate, callback);
        }

        public static void Send(this NetworkConnection connection, PeerMessageFactory data, RC4 key)
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

            public override NetworkOutgoingMessage GetMessage()
            {
                return decrypted.GetMessage().Encrypt(key);
            }
        }
    }
}