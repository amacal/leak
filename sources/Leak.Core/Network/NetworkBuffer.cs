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
        private readonly Socket socket;
        private readonly byte[] data;
        private readonly NetworkBufferConfiguration configuration;

        private int offset;
        private int length;

        /// <summary>
        /// Creates a new instance of the network buffer relying on the already
        /// connected socket instance and configuration defining the buffer size
        /// and how the incoming data should be decrypted.
        /// </summary>
        /// <param name="socket">The already connected socket.</param>
        /// <param name="configurer">The configurer to parametrize newly created instance.</param>
        public NetworkBuffer(Socket socket, Action<NetworkBufferConfiguration> configurer)
        {
            this.socket = socket;
            this.configuration = new NetworkBufferConfiguration
            {
                Size = 40000,
                Decryptor = NetworkBufferDecryptor.Nothing
            };

            configurer.Invoke(configuration);
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
            socket = buffer.socket;
            data = buffer.data;
            length = buffer.length;
            offset = buffer.offset;

            configuration = new NetworkBufferConfiguration
            {
                Size = buffer.configuration.Size,
                Decryptor = buffer.configuration.Decryptor
            };

            configurer.Invoke(configuration);
            Decrypt(offset, length);
        }

        /// <summary>
        /// The current number of bytes waiting in the buffer.
        /// </summary>
        public int Length
        {
            get { return length; }
        }

        /// <summary>
        /// Begins receiving data from the remote endpoint. If the buffer already
        /// contains data it will wait anyway for additional remote data. The handler
        /// will be notified in asynchronous way.
        /// </summary>
        /// <param name="handler">An instance of the incoming message handler.</param>
        public void Receive(NetworkIncomingMessageHandler handler)
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
                handler.OnException(ex);
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
            if (length > 0)
            {
                handler.BeginOnMessage(new NetworkIncomingMessage(this));
            }
            else
            {
                Receive(handler);
            }
        }

        private void OnReceived(IAsyncResult result)
        {
            var handler = (NetworkIncomingMessageHandler)result.AsyncState;

            try
            {
                int received = socket.EndReceive(result);

                if (received > 0)
                {
                    Decrypt(offset, received);
                    length += received;

                    handler.OnMessage(new NetworkIncomingMessage(this));
                }
                else
                {
                    handler.OnDisconnected();
                }
            }
            catch (SocketException ex)
            {
                handler.OnException(ex);
            }
        }

        public void Remove(int bytes)
        {
            offset = (offset + bytes) % configuration.Size;
            length = length - bytes;
        }

        public byte[] ToBytes()
        {
            byte[] result = new byte[length];
            int min = Math.Min(length, configuration.Size - offset);

            Array.Copy(data, offset, result, 0, min);
            Array.Copy(data, 0, result, min, length - min);

            return result;
        }

        private void Decrypt(int offset, int count)
        {
            int min = Math.Min(count, configuration.Size - offset);

            byte[] first = new byte[min];
            byte[] second = new byte[count - min];

            Array.Copy(data, offset, first, 0, min);
            Array.Copy(data, 0, second, 0, count - min);

            first = configuration.Decryptor.Decrypt(first);
            second = configuration.Decryptor.Decrypt(second);

            Array.Copy(first, 0, data, offset, min);
            Array.Copy(second, 0, data, 0, count - min);
        }
    }
}