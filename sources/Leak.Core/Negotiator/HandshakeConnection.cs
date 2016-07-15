using Leak.Core.Network;
using System;

namespace Leak.Core.Negotiator
{
    public class HandshakeConnection
    {
        private readonly NetworkConnection connection;
        private readonly HandshakeNegotiatorContext context;

        public HandshakeConnection(NetworkConnection connection, HandshakeNegotiatorContext context)
        {
            this.connection = connection;
            this.context = context;
        }

        public void Receive(Action<NetworkIncomingMessage> callback)
        {
            connection.Receive(new HandshakeConnectionToCall(context, callback));
        }

        public void Receive(Action<NetworkIncomingMessage> callback, int bytes)
        {
            connection.Receive(new HandshakeConnectionToBytes(context, callback, bytes));
        }

        public void Receive(Action<NetworkIncomingMessage> callback, Func<NetworkIncomingMessage, bool> peek)
        {
            connection.Receive(new HandshakeConnectionToPeek(context, callback, peek));
        }

        public void Send(NetworkOutgoingMessage message)
        {
            connection.Send(message);
        }

        public void Send(NetworkOutgoingMessage message, HandshakeKey key)
        {
            connection.Send(new HandshakeConnectionEncryptedMessage(message, key));
        }

        public void Close()
        {
            connection.Close();
        }

        public NetworkConnection StartEncryption(HandshakeKeyContainer pair)
        {
            return new NetworkConnection(connection, with =>
            {
                with.Encryptor = new HandshakeConnectionToEncryptor(pair.Local);
                with.Decryptor = new HandshakeConnectionToDecryptor(pair.Remote);
            });
        }
    }
}