using Leak.Common;
using Leak.Sockets;
using System;
using System.Net;

namespace Leak.Networking
{
    /// <summary>
    /// Describes the network connection between local endpoint and
    /// remote endpoint initiated by one of the side.
    /// </summary>
    public class NetworkPoolConnection : NetworkConnection, IDisposable
    {
        private readonly TcpSocket socket;
        private readonly PeerAddress remote;
        private readonly long identifier;

        private readonly NetworkBuffer buffer;
        private readonly NetworkDirection direction;
        private readonly NetworkPoolListener listener;
        private readonly NetworkEncryptor encryptor;

        /// <summary>
        /// Creates a new instance of the network connection relying on the
        /// already connected socket instance and direction value indicating
        /// who is the initiator of the connection.
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="socket">The already connected socket.</param>
        /// <param name="direction">The direction indicating who initiated the connection.</param>
        /// <param name="identifier"></param>
        public NetworkPoolConnection(NetworkPoolListener listener, TcpSocket socket, NetworkDirection direction, long identifier, IPEndPoint remote)
        {
            this.listener = listener;
            this.socket = socket;
            this.direction = direction;
            this.identifier = identifier;

            this.remote = PeerAddress.Parse(remote);

            buffer = new NetworkBuffer(listener, socket, identifier);
            encryptor = NetworkEncryptor.Nothing;
        }

        /// <summary>
        /// Creates a new instance of the network connection from the existing
        /// instance. The inner socket and direction will copied, but the caller
        /// can decide how the encryption and decryption will work.
        /// </summary>
        /// <param name="connection">The existing instance of the connection.</param>
        /// <param name="configurer">The new configuration.</param>
        public NetworkPoolConnection(NetworkPoolConnection connection, NetworkConfiguration configurer)
        {
            encryptor = configurer.Encryptor;
            listener = connection.listener;
            socket = connection.socket;
            remote = connection.remote;
            direction = connection.direction;
            identifier = connection.identifier;

            buffer = new NetworkBuffer(connection.buffer, configurer.Decryptor);
        }

        public long Identifier
        {
            get { return identifier; }
        }

        /// <summary>
        /// Gets a text representation of the remote endpoint.
        /// </summary>
        public PeerAddress Remote
        {
            get { return remote; }
        }

        /// <summary>
        /// Indicates who is the initiator of the connection.
        /// </summary>
        public NetworkDirection Direction
        {
            get { return direction; }
        }

        /// <summary>
        /// Begins receiving the incoming message by handler as
        /// an asynchronous call. It will not block the caller.
        /// </summary>
        /// <param name="handler">An instance of the incoming message handler.</param>
        public void Receive(NetworkIncomingMessageHandler handler)
        {
            buffer.ReceiveOrCallback(handler);
        }

        /// <summary>
        /// Sends the outgoing message to the remote endpoint.
        /// </summary>
        /// <param name="message">An instance of the outgoing message.</param>
        public void Send(NetworkOutgoingMessage message)
        {
            if (listener.IsAvailable(identifier))
            {
                byte[] decrypted = message.ToBytes();
                byte[] encrypted = encryptor.Encrypt(decrypted);

                listener.Schedule(new NetworkPoolSend(listener, identifier, socket, encrypted));
            }
        }

        /// <summary>
        /// Terminates the connection with the remote endpoint.
        /// </summary>
        public void Terminate()
        {
            if (listener.IsAvailable(identifier))
            {
                listener.Disconnect(identifier);
            }
        }

        public void Dispose()
        {
            socket.Dispose();
            buffer.Dispose();
        }
    }
}