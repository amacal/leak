using System;

namespace Leak.Files
{
    public class FileBuffer
    {
        private readonly byte[] data;
        private readonly int offset;
        private readonly int count;

        public FileBuffer(int size)
        {
            this.data = new byte[size];
            this.offset = 0;
            this.count = size;
        }

        public FileBuffer(byte[] data)
        {
            this.data = data;
            this.offset = 0;
            this.count = data.Length;
        }

        public FileBuffer(byte[] data, int offset)
        {
            this.data = data;
            this.offset = offset;
            this.count = data.Length - offset;
        }

        public FileBuffer(byte[] data, int offset, int count)
        {
            this.data = data;
            this.offset = offset;
            this.count = count;
        }

        public byte[] Data
        {
            get { return data; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public int Count
        {
            get { return count; }
        }

        public static implicit operator FileBuffer(byte[] data)
        {
            return new FileBuffer(data);
        }

        public byte[] ToBytes(int size)
        {
            if (offset == 0 && count == size)
                return data;

            byte[] result = new byte[size];
            Array.Copy(data, offset, result, 0, size);
            return result;
        }
    }
}