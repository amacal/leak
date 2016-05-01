namespace Leak.Core.Encoding
{
    public class BencodedNumber : BencodedValue
    {
        private readonly long value;

        public BencodedNumber(long value)
        {
            this.value = value;
        }

        public long Value
        {
            get { return value; }
        }
    }
}