using Leak.Sockets;
using System;
using Leak.Networking.Core;

namespace Leak.Networking
{
    /// <summary>
    /// Describes the network socket wrapper with built-in data buffering
    /// designed only to receive data from the remote endpoint.
    /// </summary>
    internal class NetworkIncomingBuffer
    {
        private readonly NetworkPoolListener listener;
        private readonly NetworkIncomingDecryptor decryptor;

        private readonly TcpSocket socket;
        private readonly long identifier;
        private readonly NetworkPoolMemoryBlock memory;

        private int offset;
        private int length;

        /// <summary>
        /// Creates a new instance of the network incoming buffer relying on the already
        /// connected socket instance. The buffer does not decrypt anything.
        /// </summary>
        /// <param name="listener">The listener who knows the pool.</param>
        /// <param name="socket">The already connected socket.</param>
        /// <param name="identifier">The unique connection identifier.</param>
        public NetworkIncomingBuffer(NetworkPoolListener listener, TcpSocket socket, long identifier)
        {
            this.listener = listener;
            this.socket = socket;
            this.identifier = identifier;

            memory = listener.Allocate();
        }

        /// <summary>
        /// Creates a new instance of the network incoming buffer from the existing instance.
        /// The inner socket and the already downloaded and waiting data will be
        /// copied, but the caller can change the decryption algorithm.
        /// </summary>
        /// <param name="buffer">The existing instance of the newtwork buffer.</param>
        /// <param name="decryptor">The new decryptor.</param>
        public NetworkIncomingBuffer(NetworkIncomingBuffer buffer, NetworkIncomingDecryptor decryptor)
        {
            this.decryptor = decryptor;

            listener = buffer.listener;
            socket = buffer.socket;
            identifier = buffer.identifier;
            memory = buffer.memory;
            length = buffer.length;
            offset = buffer.offset;

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
                int receiveOffset;
                int receiveSize;

                if (offset + length >= memory.Length)
                {
                    receiveOffset = offset + length - memory.Length;
                    receiveSize = offset - (offset + length) % memory.Length;
                }
                else
                {
                    receiveOffset = offset + length;
                    receiveSize = memory.Length - offset - length;
                }

                socket.Receive(new SocketBuffer(memory.Data, receiveOffset, receiveSize), context => OnReceived(context, handler));
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
                    listener.Schedule(new NetworkPoolReceive(handler, new NetworkIncomingBufferMessage(this)));
                }
                else
                {
                    Receive(handler);
                }
            }
        }

        private void OnReceived(TcpSocketReceive received, NetworkIncomingMessageHandler handler)
        {
            listener.Schedule(new NetworkPoolDecrypt(listener, identifier, this, handler, received.Count));
        }

        public void Process(NetworkIncomingMessageHandler handler, int count)
        {
            if (listener.IsAvailable(identifier))
            {
                if (count > 0)
                {
                    if (offset + length >= memory.Length)
                    {
                        Decrypt(offset + length - memory.Length, count);
                    }
                    else
                    {
                        Decrypt(offset + length, count);
                    }

                    length += count;
                    handler.OnMessage(new NetworkIncomingBufferMessage(this));
                }
                else
                {
                    listener.Disconnect(identifier);
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

            offset = (offset + bytes) % memory.Length;
            length = length - bytes;
        }

        private void Decrypt(int start, int count)
        {
            int min = Math.Min(count, memory.Length - start);

            decryptor?.Decrypt(memory.Data, start, min);
            decryptor?.Decrypt(memory.Data, 0, count - min);
        }

        public NetworkIncomingBufferView View()
        {
            return new NetworkIncomingBufferView(memory.Data, length, offset);
        }

        public void Dispose()
        {
            memory.Release();
        }
    }
}