namespace Leak.Core.Encoding
{
    public class BencodedNumber
    {
        private readonly long value;

        public BencodedNumber(long value)
        {
            this.value = value;
        }

        public long ToInt64()
        {
            return value;
        }
    }
}