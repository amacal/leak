namespace Leak.Core.Bencoding
{
    public class BencodedText
    {
        private readonly BencodedData data;

        public BencodedText(string value)
        {
            this.data = new BencodedData(value);
        }

        public BencodedText(BencodedData data)
        {
            this.data = data;
        }

        public int Length
        {
            get { return data.Length; }
        }

        public string GetString()
        {
            return System.Text.Encoding.ASCII.GetString(data.GetBytes()); ;
        }

        public byte[] GetBytes()
        {
            return data.GetBytes();
        }
    }
}