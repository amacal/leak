using Leak.Sockets;
using System;
using Leak.Common;

namespace Leak.Core.Network
{
    /// <summary>
    /// Describes the network socket wrapper with built-in data buffering
    /// designed only to receive data from the remote endpoint.
    /// </summary>
    public class NetworkBuffer
    {
        private readonly NetworkPoolListener listener;
        private readonly TcpSocket socket;
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
        public NetworkBuffer(NetworkPoolListener listener, TcpSocket socket, long identifier, Action<NetworkBufferConfiguration> configurer)
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
                lock (configuration.Synchronized)
                {
                    int receiveOffset;
                    int receiveSize;

                    if (offset + length >= configuration.Size)
                    {
                        receiveOffset = offset + length - configuration.Size;
                        receiveSize = offset - (offset + length) % configuration.Size;
                    }
                    else
                    {
                        receiveOffset = offset + length;
                        receiveSize = configuration.Size - offset - length;
                    }

                    socket.Receive(new TcpSocketBuffer(data, receiveOffset, receiveSize), context => OnReceived(context, handler));
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
                    listener.Schedule(new NetworkPoolReceive(handler, new NetworkBufferMessage(this)));
                }
                else
                {
                    Receive(handler);
                }
            }
        }

        private void OnReceived(TcpSocketReceive context, NetworkIncomingMessageHandler handler)
        {
            listener.Schedule(new NetworkPoolDecrypt(this, handler, context.Count));
        }

        public void Process(NetworkIncomingMessageHandler handler, int count)
        {
            if (listener.IsAvailable(identifier))
            {
                if (count > 0)
                {
                    if (offset + length >= configuration.Size)
                    {
                        Decrypt(offset + length - configuration.Size, count);
                    }
                    else
                    {
                        Decrypt(offset + length, count);
                    }

                    length += count;
                    handler.OnMessage(new NetworkBufferMessage(this));
                }
                else
                {
                    listener.OnDisconnected(identifier);
                    handler.OnDisconnected();
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