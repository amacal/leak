using System;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerBuffer
    {
        private readonly byte[] data;
        private readonly int size;

        private int offset;
        private int count;

        public PeerBuffer(int size)
        {
            this.size = size;
            this.data = new byte[size];
        }

        public int Count
        {
            get { return count; }
        }

        public void Receive(Socket socket, Action<PeerMessage> callback)
        {
            if (offset + count >= size)
            {
                socket.BeginReceive(data, (offset + count) - size, offset - ((offset + count) % size), SocketFlags.None, OnReceived, Tuple.Create(socket, callback));
            }
            else
            {
                socket.BeginReceive(data, offset + count, size - offset - count, SocketFlags.None, OnReceived, Tuple.Create(socket, callback));
            }
        }

        public void ReceiveOrCallback(Socket socket, Action<PeerMessage> callback)
        {
            if (count > 0)
            {
                callback.BeginInvoke(new PeerMessage(this), null, null);
            }
            else
            {
                Receive(socket, callback);
            }
        }

        private void OnReceived(IAsyncResult result)
        {
            try
            {
                var state = (Tuple<Socket, Action<PeerMessage>>)result.AsyncState;
                int received = state.Item1.EndReceive(result);

                if (received > 0)
                {
                    count += received;
                    state.Item2.Invoke(new PeerMessage(this));
                }
            }
            catch (SocketException)
            {
            }
        }

        public void Remove(int length)
        {
            offset = (offset + length) % size;
            count = count - length;
        }

        public byte[] ToBytes()
        {
            byte[] result = new byte[count];
            int min = Math.Min(count, size - offset);

            Array.Copy(data, offset, result, 0, min);
            Array.Copy(data, 0, result, min, count - min);

            return result;
        }
    }
}