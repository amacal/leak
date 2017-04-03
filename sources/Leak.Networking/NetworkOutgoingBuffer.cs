using System;
using System.Threading;
using Leak.Common;
using Leak.Sockets;

namespace Leak.Networking
{
    internal class NetworkOutgoingBuffer : IDisposable
    {
        private readonly NetworkPoolListener listener;
        private readonly NetworkOutgoingEncryptor encryptor;

        private readonly TcpSocket socket;
        private readonly long identifier;
        private readonly NetworkPoolMemoryBlock memory;

        private int offset;

        /// <summary>
        /// Creates a new instance of the network outgoing buffer relying on the already
        /// connected socket instance. The buffer does not encrypt anything.
        /// </summary>
        /// <param name="listener">The listener who knows the pool.</param>
        /// <param name="socket">The already connected socket.</param>
        /// <param name="identifier">The unique connection identifier.</param>
        public NetworkOutgoingBuffer(NetworkPoolListener listener, TcpSocket socket, long identifier)
        {
            this.listener = listener;
            this.socket = socket;
            this.identifier = identifier;

            memory = listener.Allocate();
        }

        /// <summary>
        /// Creates a new instance of the network outgoing buffer from the existing instance.
        /// The inner socket and the already downloaded and waiting data will be
        /// copied, but the caller can change the encryption algorithm.
        /// </summary>
        /// <param name="buffer">The existing instance of the newtwork buffer.</param>
        /// <param name="encryptor">The new encryptor.</param>
        public NetworkOutgoingBuffer(NetworkOutgoingBuffer buffer, NetworkOutgoingEncryptor encryptor)
        {
            this.encryptor = encryptor;

            listener = buffer.listener;
            socket = buffer.socket;
            identifier = buffer.identifier;
            memory = buffer.memory;
            offset = buffer.offset;

            encryptor?.Encrypt(memory.Data, 0, offset);
        }

        public bool IsAvailable
        {
            get { return offset == 0; }
        }

        public void Send(NetworkOutgoingMessage message, Action callback)
        {
            if (listener.IsAvailable(identifier))
            {
                DataBlock block = new NetworkOutgoingBlock(memory.Data, offset, message.Length);
                SocketBuffer buffer = new SocketBuffer(memory.Data, offset, message.Length);

                message.ToBytes(block);
                encryptor?.Encrypt(memory.Data, offset, message.Length);

                offset += message.Length;
                socket.Send(buffer, OnSent(callback));
            }
        }

        private TcpSocketSendCallback OnSent(Action callback)
        {
            return sent =>
            {
                listener.Schedule(() =>
                {
                    if (listener.IsAvailable(identifier))
                    {
                        if (sent.Status != SocketStatus.OK || sent.Count == 0)
                        {
                            listener.Disconnect(identifier);
                        }

                        if (sent.Count > 0)
                        {
                            listener.HandleSent(identifier, sent.Count);
                        }
                    }

                    offset = 0;
                    callback.Invoke();
                });
            };
        }

        public void Dispose()
        {
            memory.Release();
        }
    }
}