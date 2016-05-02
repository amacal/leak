namespace Leak.Core.Encoding
{
    public class BencodedText : BencodedValue
    {
        private readonly string value;

        public BencodedText(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }
}