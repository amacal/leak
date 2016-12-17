using System;

namespace Leak.Bencoding
{
    public class BencodedNumber
    {
        private readonly long value;

        public BencodedNumber(long value)
        {
            this.value = value;
        }

        public byte ToByte()
        {
            return Convert.ToByte(value);
        }

        public int ToInt32()
        {
            return Convert.ToInt32(value);
        }

        public long ToInt64()
        {
            return value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}