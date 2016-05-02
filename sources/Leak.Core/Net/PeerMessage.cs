using System;

namespace Leak.Core.Net
{
    public class PeerMessage
    {
        private readonly byte[] data;
        private readonly int length;

        public PeerMessage(params byte[] data)
        {
            this.data = data;
            this.length = data.Length;
        }

        public PeerMessage(PeerBuffer buffer)
        {
            this.length = buffer.Count;
            this.data = buffer.ToBytes();
        }

        public byte this[int index]
        {
            get { return data[index]; }
        }

        public int Length
        {
            get { return length; }
        }

        public byte[] ToBytes()
        {
            return data;
        }

        public byte[] ToBytes(int offset)
        {
            byte[] result = new byte[data.Length - offset];
            Array.Copy(data, offset, result, 0, result.Length);
            return result;
        }

        public byte[] ToBytes(int offset, int count)
        {
            byte[] result = new byte[count];
            Array.Copy(data, offset, result, 0, result.Length);
            return result;
        }
    }
}