using System;

namespace Leak.Core.Encoding
{
    public abstract class BencodedValue
    {
        private byte[] data;
        private int start;
        private int end;

        public void SetSource(byte[] data, int start, int end)
        {
            this.data = data;
            this.start = start;
            this.end = end;
        }

        public byte[] ToHex()
        {
            byte[] result = new byte[end - start + 1];
            Array.Copy(data, start, result, 0, result.Length);

            return result;
        }
    }
}