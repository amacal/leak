using Leak.Suckets;
using System;
using System.Net;

namespace Leak.Core.Network
{
    /// <summary>
    /// Describes the network connection between local endpoint and
    /// remote endpoint initiated by one of the side.
    /// </summary>
    public class NetworkPoolConnection : NetworkConnection, IDisposable
    {
        private readonly TcpSocket socket;
        private readonly string remote;
        private readonly long identifier;
        private readonly object synchronized;

        private readonly NetworkBuffer buffer;
        private readonly NetworkDirection direction;
        private readonly NetworkPoolListener listener;
        private readonly NetworkConnectionConfiguration configuration;

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

            this.configuration = new NetworkConnectionConfiguration
            {
                Encryptor = NetworkConnectionEncryptor.Nothing,
                Decryptor = NetworkConnectionDecryptor.Nothing,
            };

            this.remote = remote.ToString();
            this.synchronized = new object();

            this.buffer = new NetworkBuffer(listener, socket, identifier, with =>
            {
                with.Size = 40000;
                with.Decryptor = NetworkBufferDecryptor.Nothing;
                with.Synchronized = synchronized;
            });
        }

        /// <summary>
        /// Creates a new instance of the network connection from the existing
        /// instance. The inner socket and direction will copied, but the caller
        /// can decide how the encryption and decryption will work.
        /// </summary>
        /// <param name="connection">The existing instance of the connection.</param>
        /// <param name="configurer">The configurer to parametrize newly created instance.</param>
        public NetworkPoolConnection(NetworkPoolConnection connection, Action<NetworkConnectionConfiguration> configurer)
        {
            listener = connection.listener;
            socket = connection.socket;
            remote = connection.remote;
            direction = connection.direction;
            identifier = connection.identifier;
            synchronized = connection.synchronized;

            configuration = configurer.Configure(with =>
            {
                with.Encryptor = connection.configuration.Encryptor;
                with.Decryptor = connection.configuration.Decryptor;
            });

            buffer = new NetworkBuffer(connection.buffer, with =>
            {
                with.Decryptor = new NetworkConnectionDecryptorToBuffer(configuration.Decryptor);
            });
        }

        public long Identifier
        {
            get { return identifier; }
        }

        /// <summary>
        /// Gets a text representation of the remote endpoint.
        /// </summary>
        public string Remote
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
        /// Sending always blocks and is executed in the current thread.
        /// </summary>
        /// <param name="message">An instance of the outgoing message.</param>
        public void Send(NetworkOutgoingMessage message)
        {
            if (listener.IsAvailable(identifier))
            {
                lock (synchronized)
                {
                    byte[] decrypted = message.ToBytes();
                    byte[] encrypted = configuration.Encryptor.Encrypt(decrypted);

                    listener.Schedule(new NetworkPoolSend(identifier, socket, encrypted));
                }
            }
        }

        /// <summary>
        /// Terminates the connection with the remote endpoint.
        /// </summary>
        public void Terminate()
        {
            if (listener.IsAvailable(identifier))
            {
                listener.OnDisconnected(identifier);
            }
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }
}