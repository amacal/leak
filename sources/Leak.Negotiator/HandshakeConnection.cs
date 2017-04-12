using System;
using Leak.Common;
using Leak.Networking;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeConnection
    {
        private readonly NetworkPool pool;
        private readonly NetworkConnection connection;
        private readonly HandshakeNegotiatorHooks hooks;

        public HandshakeConnection(NetworkPool pool, NetworkConnection connection, HandshakeNegotiatorHooks hooks)
        {
            this.pool = pool;
            this.connection = connection;
            this.hooks = hooks;
        }

        public void Receive(Action<NetworkIncomingMessage> callback, int bytes)
        {
            connection.ReceiveIf(callback, bytes);
        }

        public void Receive<T>(Action<NetworkIncomingMessage, T> callback, int bytes, T parameter)
        {
            connection.ReceiveIf(x => callback.Invoke(x, parameter), bytes);
        }

        public void Receive(Action<NetworkIncomingMessage> callback, Func<NetworkIncomingMessage, bool> peek)
        {
            connection.ReceiveIf(callback, peek, CallHandshakeRejected);
        }

        public void Send(NetworkOutgoingMessage message)
        {
            connection.Send(message);
        }

        public void Send(NetworkOutgoingMessage message, HandshakeKey key)
        {
            connection.Send(new HandshakeConnectionEncryptedMessage(message, key));
        }

        public void CallHandshakeCompleted(Handshake handshake)
        {
            hooks.CallHandshakeCompleted(connection, handshake);
        }

        public void CallHandshakeRejected()
        {
            hooks.CallHandshakeRejected(connection);
        }

        public void Terminate()
        {
            connection.Terminate();
        }

        public NetworkConnection StartEncryption(HandshakeKeyContainer pair)
        {
            return pool.Change(connection, new NetworkConfiguration
            {
                Encryptor = new HandshakeConnectionToEncryptor(pair.Local),
                Decryptor = new HandshakeConnectionToDecryptor(pair.Remote)
            });
        }
    }
}