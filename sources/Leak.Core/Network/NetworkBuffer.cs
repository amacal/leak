using System;
using System.Net.Sockets;

namespace Leak.Core.Network
{
    /// <summary>
    /// Describes the network socket wrapper with built-in data buffering
    /// designed only to receive data from the remote endpoint.
    /// </summary>
    public class NetworkBuffer
    {
        private readonly NetworkPoolListener listener;
        private readonly Socket socket;
        private readonly long identifier;
        private readonly byte[] data;
        private readonly NetworkBufferConfiguration configuration;

        private int offset;
        private int length;

        /// <summary>
        /// Creates a new instance of the network buffer relying on the already
        /// connected socket instance and configuration defining the buffer size
        /// and how the incoming data should be decrypted.
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="socket">The already connected socket.</param>
        /// <param name="identifier"></param>
        /// <param name="configurer">The configurer to parametrize newly created instance.</param>
        public NetworkBuffer(NetworkPoolListener listener, Socket socket, long identifier, Action<NetworkBufferConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Size = 40000;
                with.Decryptor = NetworkBufferDecryptor.Nothing;
                with.Synchronized = new object();
            });

            this.listener = listener;
            this.socket = socket;
            this.identifier = identifier;

            this.data = new byte[configuration.Size];
        }

        /// <summary>
        /// Creates a new instance of the network buffer from the existing instance.
        /// The inner socket and the already downloaded and waiting data will be
        /// copied, but the caller can change the buffer size and decryption algorithm.
        /// </summary>
        /// <param name="buffer">The existing instance of the newtwork buffer.</param>
        /// <param name="configurer">The configurer to parametrize newly created instance.</param>
        public NetworkBuffer(NetworkBuffer buffer, Action<NetworkBufferConfiguration> configurer)
        {
            listener = buffer.listener;
            socket = buffer.socket;
            identifier = buffer.identifier;
            data = buffer.data;
            length = buffer.length;
            offset = buffer.offset;

            configuration = configurer.Configure(with =>
            {
                with.Size = buffer.configuration.Size;
                with.Decryptor = buffer.configuration.Decryptor;
                with.Synchronized = buffer.configuration.Synchronized;
            });

            Decrypt(offset, length);
        }

        /// <summary>
        /// Begins receiving data from the remote endpoint. If the buffer already
        /// contains data it will wait anyway for additional remote data. The handler
        /// will be notified in asynchronous way.
        /// </summary>
        /// <param name="handler">An instance of the incoming message handler.</param>
        public void Receive(NetworkIncomingMessageHandler handler)
        {
            if (listener.IsAvailable(identifier))
            {
                try
                {
                    if (offset + length >= configuration.Size)
                    {
                        socket.BeginReceive(data, offset + length - configuration.Size, offset - (offset + length) % configuration.Size, SocketFlags.None, OnReceived, handler);
                    }
                    else
                    {
                        socket.BeginReceive(data, offset + length, configuration.Size - offset - length, SocketFlags.None, OnReceived, handler);
                    }
                }
                catch (SocketException ex)
                {
                    listener.OnException(identifier, ex);
                    handler.OnException(ex);
                }
            }
        }

        /// <summary>
        /// Begins receiving data first from the local buffer. If nothing is available
        /// from the remote endpoint. In both cases the caller will be notified in
        /// asynchronous way.
        /// </summary>
        /// <param name="handler">An instance of the incoming message handler.</param>
        public void ReceiveOrCallback(NetworkIncomingMessageHandler handler)
        {
            if (listener.IsAvailable(identifier))
            {
                if (length > 0)
                {
                    handler.BeginOnMessage(new NetworkBufferMessage(this));
                }
                else
                {
                    Receive(handler);
                }
            }
        }

        private void OnReceived(IAsyncResult result)
        {
            var handler = (NetworkIncomingMessageHandler)result.AsyncState;

            if (listener.IsAvailable(identifier))
            {
                try
                {
                    int received = 0;

                    lock (configuration.Synchronized)
                    {
                        received = socket.EndReceive(result);
                    }

                    if (received > 0)
                    {
                        if (offset + length >= configuration.Size)
                        {
                            Decrypt(offset + length - configuration.Size, received);
                        }
                        else
                        {
                            Decrypt(offset + length, received);
                        }

                        length += received;
                        handler.OnMessage(new NetworkBufferMessage(this));
                    }
                    else
                    {
                        listener.OnDisconnected(identifier);
                        handler.OnDisconnected();
                    }
                }
                catch (SocketException ex)
                {
                    listener.OnException(identifier, ex);
                    handler.OnException(ex);
                }
            }
        }

        public void Remove(int bytes)
        {
            if (bytes > length)
            {
                throw new InvalidOperationException();
            }

            offset = (offset + bytes) % configuration.Size;
            length = length - bytes;
        }

        private void Decrypt(int start, int count)
        {
            int min = Math.Min(count, configuration.Size - start);

            configuration.Decryptor.Decrypt(data, start, min);
            configuration.Decryptor.Decrypt(data, 0, count - min);
        }

        public NetworkBufferView View()
        {
            return new NetworkBufferView(data, length, offset);
        }
    }
}