using System;
using Leak.Networking.Core;

namespace Leak.Networking
{
    public class NetworkIncomingBufferView
    {
        private readonly byte[] data;
        private readonly int length;
        private readonly int offset;

        public NetworkIncomingBufferView(byte[] data, int length, int offset)
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

        public byte[] ToBytes(byte[] result, int position, int start, int count)
        {
            int min = Math.Min(length - start, data.Length - offset - start);

            if (min > 0)
            {
                Array.Copy(data, offset + start, result, position, Math.Min(min, count));
                Array.Copy(data, 0, result, Math.Min(min, count) + position, count - Math.Min(min, count));
            }
            else
            {
                Array.Copy(data, -min, result, position, count);
            }

            return result;
        }

        public byte[] ToBytes(int start, int count)
        {
            return ToBytes(new byte[count], 0, start, count);
        }

        public DataBlock ToBlock(DataBlockFactory factory, int start, int count)
        {
            return factory.Pooled(count, (x, y, z) => ToBytes(x, y, start, count));
        }
    }
}