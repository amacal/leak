namespace Leak.Bencoding
{
    public class BencodedValue
    {
        public BencodedData Data { get; set; }

        public BencodedText Text { get; set; }

        public BencodedNumber Number { get; set; }

        public BencodedValue[] Array { get; set; }

        public BencodedEntry[] Dictionary { get; set; }

        public override string ToString()
        {
            if (Text != null)
            {
                return Text.GetString();
            }

            return base.ToString();
        }
    }
}