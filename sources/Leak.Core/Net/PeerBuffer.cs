using System;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerBuffer
    {
        private readonly Socket socket;
        private readonly byte[] data;
        private readonly PeerBufferConfiguration configuration;

        private int offset;
        private int count;

        public PeerBuffer(Socket socket, Action<PeerBufferConfiguration> configurer)
        {
            this.socket = socket;
            this.configuration = new PeerBufferConfiguration
            {
                Size = 40000,
                Decrypt = data => data
            };

            configurer.Invoke(configuration);
            this.data = new byte[configuration.Size];
        }

        public PeerBuffer(PeerBuffer buffer, Action<PeerBufferConfiguration> configurer)
        {
            socket = buffer.socket;
            data = buffer.data;
            count = buffer.count;
            offset = buffer.offset;

            configuration = new PeerBufferConfiguration
            {
                Size = buffer.configuration.Size,
                Decrypt = buffer.configuration.Decrypt
            };

            configurer.Invoke(configuration);
            Decrypt(offset, count);
        }

        public int Count
        {
            get { return count; }
        }

        public void Receive(Action<PeerMessage> callback)
        {
            if (offset + count >= configuration.Size)
            {
                socket.BeginReceive(data, (offset + count) - configuration.Size, offset - ((offset + count) % configuration.Size), SocketFlags.None, OnReceived, callback);
            }
            else
            {
                socket.BeginReceive(data, offset + count, configuration.Size - offset - count, SocketFlags.None, OnReceived, callback);
            }
        }

        public void ReceiveOrCallback(Action<PeerMessage> callback)
        {
            if (count > 0)
            {
                callback.BeginInvoke(new PeerMessage(this), null, null);
            }
            else
            {
                Receive(callback);
            }
        }

        private void OnReceived(IAsyncResult result)
        {
            try
            {
                var callback = (Action<PeerMessage>)result.AsyncState;
                int received = socket.EndReceive(result);

                if (received > 0)
                {
                    Decrypt(offset, received);
                    count += received;
                    callback.Invoke(new PeerMessage(this));
                }
            }
            catch (SocketException)
            {
            }
        }

        public void Remove(int length)
        {
            offset = (offset + length) % configuration.Size;
            count = count - length;
        }

        public byte[] ToBytes()
        {
            byte[] result = new byte[count];
            int min = Math.Min(count, configuration.Size - offset);

            Array.Copy(data, offset, result, 0, min);
            Array.Copy(data, 0, result, min, count - min);

            return result;
        }

        private void Decrypt(int offset, int count)
        {
            int min = Math.Min(count, configuration.Size - offset);

            byte[] first = new byte[min];
            byte[] second = new byte[count - min];

            Array.Copy(data, offset, first, 0, min);
            Array.Copy(data, 0, second, 0, count - min);

            first = configuration.Decrypt.Invoke(first);
            second = configuration.Decrypt.Invoke(second);

            Array.Copy(first, 0, data, offset, min);
            Array.Copy(second, 0, data, 0, count - min);
        }
    }
}