using System;

namespace Leak.Core.Network
{
    public class NetworkBufferView
    {
        private readonly byte[] data;
        private readonly int length;
        private readonly int offset;

        public NetworkBufferView(byte[] data, int length, int offset)
        {
            this.data = data;
            this.length = length;
            this.offset = offset;
        }

        /// <summary>
        /// The current number of bytes waiting in the buffer.
        /// </summary>
        public int Length
        {
            get { return length; }
        }

        public byte this[int index]
        {
            get { return data[(offset + index) % data.Length]; }
        }

        public byte[] ToBytes()
        {
            byte[] result = new byte[length];
            int min = Math.Min(length, data.Length - offset);

            Array.Copy(data, offset, result, 0, min);
            Array.Copy(data, 0, result, min, length - min);

            return result;
        }

        public byte[] ToBytes(int start)
        {
            byte[] result = new byte[length - start];
            int min = Math.Min(length - start, data.Length - offset - start);

            if (min > 0)
            {
                Array.Copy(data, offset + start, result, 0, min);
                Array.Copy(data, 0, result, min, length - start - min);
            }
            else
            {
                Array.Copy(data, -min, result, 0, length - start);
            }

            return result;
        }

        public byte[] ToBytes(int start, int count)
        {
            byte[] result = new byte[count];
            int min = Math.Min(length - start, data.Length - offset - start);

            if (min > 0)
            {
                Array.Copy(data, offset + start, result, 0, Math.Min(min, count));
                Array.Copy(data, 0, result, Math.Min(min, count), count - Math.Min(min, count));
            }
            else
            {
                Array.Copy(data, -min, result, 0, count);
            }

            return result;
        }
    }
}