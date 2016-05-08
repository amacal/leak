using System;

namespace Leak.Core
{
    public static class BytesExtensions
    {
        public static byte[] ToBytes(this byte[] data, int offset)
        {
            byte[] result = new byte[data.Length - offset];
            Array.Copy(data, offset, result, 0, result.Length);
            return result;
        }

        public static byte[] ToBytes(this byte[] data, int offset, int count)
        {
            byte[] result = new byte[count];
            Array.Copy(data, offset, result, 0, result.Length);
            return result;
        }

        public static byte[] Encrypt(this byte[] data, RC4 key)
        {
            return key.Encrypt(data);
        }
    }
}