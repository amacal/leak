using Leak.Sockets;
using System;
using System.Net;
using Leak.Networking.Core;

namespace Leak.Networking
{
    /// <summary>
    /// Describes the network connection between local endpoint and
    /// remote endpoint initiated by one of the side.
    /// </summary>
    public class NetworkPoolConnection : NetworkConnection, IDisposable
    {
        private readonly long identifier;
        private readonly TcpSocket socket;
        private readonly NetworkAddress remote;

        private readonly NetworkIncomingBuffer incoming;
        private readonly NetworkOutgoingBuffer outgoing;

        private readonly NetworkDirection direction;
        private readonly NetworkPoolListener listener;

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

            this.remote = NetworkAddress.Parse(remote);

            incoming = new NetworkIncomingBuffer(listener, socket, identifier);
            outgoing = new NetworkOutgoingBuffer(listener, socket, identifier);
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
            listener = connection.listener;
            socket = connection.socket;
            remote = connection.remote;
            direction = connection.direction;
            identifier = connection.identifier;

            incoming = new NetworkIncomingBuffer(connection.incoming, configurer.Decryptor);
            outgoing = new NetworkOutgoingBuffer(connection.outgoing, configurer.Encryptor);
        }

        public long Identifier
        {
            get { return identifier; }
        }

        /// <summary>
        /// Gets a text representation of the remote endpoint.
        /// </summary>
        public NetworkAddress Remote
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
            if (listener.IsAvailable(identifier))
            {
                incoming.ReceiveOrCallback(handler);
            }
        }

        /// <summary>
        /// Sends the outgoing message to the remote endpoint.
        /// </summary>
        /// <param name="message">An instance of the outgoing message.</param>
        public void Send(NetworkOutgoingMessage message)
        {
            if (listener.IsAvailable(identifier))
            {
                listener.Schedule(new NetworkPoolSend(identifier, outgoing, message));
            }
            else
            {
                message.Release();
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
            incoming.Dispose();
            outgoing.Dispose();
        }
    }
}