using System;

namespace Leak.Core
{
    public static class Bytes
    {
        public static byte[] Random(int size)
        {
            byte[] data = new byte[size];
            Random random = new Random();

            random.NextBytes(data);
            return data;
        }

        public static byte[] Parse(string value)
        {
            byte[] result = new byte[value.Length / 2];

            for (int i = 0; i < value.Length; i += 2)
            {
                result[i / 2] = (byte)(ToByte(value[i]) * 16 + ToByte(value[i + 1]));
            }

            return result;
        }

        private static int ToByte(char value)
        {
            if (value >= '0' && value <= '9')
                return value - '0';

            if (value >= 'a' && value <= 'f')
                return value - 'a' + 10;

            if (value >= 'A' && value <= 'F')
                return value - 'a' + 10;

            return 0;
        }
    }
}