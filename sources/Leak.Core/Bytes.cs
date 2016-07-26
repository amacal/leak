using System;
using System.Security.Cryptography;
using System.Text;

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
                return value - 'A' + 10;

            return 0;
        }

        public static byte[] Hash(string text, params byte[][] parts)
        {
            byte[] input = Encoding.ASCII.GetBytes(text);

            foreach (byte[] data in parts)
            {
                Append(ref input, data);
            }

            using (SHA1 algorithm = SHA1.Create())
            {
                return algorithm.ComputeHash(input);
            }
        }

        public static byte[] Concatenate(params byte[][] data)
        {
            byte[] result = new byte[0];

            foreach (byte[] array in data)
            {
                Append(ref result, array);
            }

            return result;
        }

        public static void Append(ref byte[] data, byte[] input)
        {
            Array.Resize(ref data, data.Length + input.Length);
            Array.Copy(input, 0, data, data.Length - input.Length, input.Length);
        }

        public static uint ReadUInt32(byte[] data, int offset)
        {
            uint value = 0;

            for (int i = 0; i < 4; i++)
            {
                value = (value << 8) + data[offset + i];
            }

            return value;
        }

        public static void Write(int value, byte[] data, int offset)
        {
            data[offset + 0] = (byte)((value >> 24) & 255);
            data[offset + 1] = (byte)((value >> 16) & 255);
            data[offset + 2] = (byte)((value >> 8) & 255);
            data[offset + 3] = (byte)(value & 255);
        }

        public static byte[] Xor(byte[] left, byte[] right)
        {
            byte[] data = new byte[left.Length];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(left[i] ^ right[i]);
            }

            return data;
        }

        public static bool Equals(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
                return false;

            for (int i = 0; i < left.Length; i++)
                if (left[i] != right[i])
                    return false;

            return true;
        }

        public static string ToString(byte[] data)
        {
            StringBuilder builder = new StringBuilder();

            foreach (byte item in data)
            {
                builder.Append(item.ToString("x2"));
            }

            return builder.ToString();
        }

        public static int Find(byte[] data, byte[] pattern)
        {
            for (int i = 0; i < data.Length - pattern.Length; i++)
            {
                bool success = true;

                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[j + i] != pattern[j])
                    {
                        success = false;
                        break;
                    }
                }

                if (success)
                {
                    return i;
                }
            }

            return -1;
        }

        public static byte[] Copy(byte[] source, int offset)
        {
            byte[] result = new byte[source.Length - offset];
            Array.Copy(source, offset, result, 0, result.Length);
            return result;
        }

        public static byte[] Copy(byte[] source, int offset, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(source, offset, result, 0, result.Length);
            return result;
        }
    }
}