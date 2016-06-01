namespace Leak.Core.Encoding
{
    public class BencodedData
    {
        private readonly byte[] data;
        private readonly int offset;
        private readonly int length;

        public BencodedData(byte[] data, int offset, int length)
        {
            this.data = data;
            this.offset = offset;
            this.length = length;
        }

        public int Length
        {
            get { return length; }
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[length];

            for (int i = 0; i < length; i++)
            {
                data[i] = this.data[offset + i];
            }

            return data;
        }
    }
}