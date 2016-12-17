namespace Leak.Bencoding
{
    public class BencodedData
    {
        private readonly byte[] data;
        private readonly int offset;
        private readonly int length;

        public BencodedData(byte[] data)
        {
            this.data = data;
            this.offset = 0;
            this.length = data.Length;
        }

        public BencodedData(byte[] data, int offset, int length)
        {
            this.data = data;
            this.offset = offset;
            this.length = length;
        }

        public BencodedData(string value)
        {
            this.offset = 0;
            this.data = System.Text.Encoding.ASCII.GetBytes(value);
            this.length = System.Text.Encoding.ASCII.GetByteCount(value);
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