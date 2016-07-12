using Leak.Core.Network;
using System;

namespace Leak.Core.Net
{
    public static class PeerConnectionExtensions
    {
        public static void Send(this NetworkConnection connection, PeerMessageFactory factory)
        {
            connection.Send(factory.GetMessage());
        }

        public static void Receive(this NetworkConnection connection, Func<NetworkIncomingMessage, bool> predicate, Action<NetworkIncomingMessage> callback)
        {
            Action<NetworkIncomingMessage> onMessage = null;

            onMessage = message =>
            {
                if (predicate.Invoke(message))
                {
                    callback.Invoke(message);
                }
                else
                {
                    message.Continue(new InlineHandler(onMessage));
                }
            };

            connection.Receive(onMessage);
        }

        public static NetworkConnection StartEncryption(this NetworkConnection connection, RC4 encryptKey, RC4 decryptKey)
        {
            return new NetworkConnection(connection, with =>
            {
                //with.Encryptor = encryptKey.Encrypt;
                //with.Decryptor = decryptKey.Decrypt;
            });
        }

        private class InlineHandler : NetworkIncomingMessageHandler
        {
            private readonly Action<NetworkIncomingMessage> handler;

            public InlineHandler(Action<NetworkIncomingMessage> handler)
            {
                this.handler = handler;
            }

            public void OnMessage(NetworkIncomingMessage message)
            {
                handler.Invoke(message);
            }

            public void OnException(Exception ex)
            {
                throw new NotImplementedException();
            }

            public void OnDisconnected()
            {
                throw new NotImplementedException();
            }
        }
    }
}