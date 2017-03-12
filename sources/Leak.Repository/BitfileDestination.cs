using System;
using System.IO;
using System.Security.Cryptography;
using Leak.Common;

namespace Leak.Data.Store
{
    public class BitfileDestination
    {
        private readonly BitfileContext context;

        public BitfileDestination(BitfileContext context)
        {
            this.context = context;
        }

        public Bitfield Read()
        {
            if (File.Exists(context.Path) == false)
                return null;

            string path = context.Path;
            byte[] data = File.ReadAllBytes(path);

            if (data.Length < 20)
                return null;

            using (HashAlgorithm algorithm = SHA1.Create())
            {
                byte[] checksum = Bytes.Copy(data, 0, 20);
                byte[] computed = algorithm.ComputeHash(data, 20, data.Length - 20);

                if (Bytes.Equals(checksum, computed) == false)
                    return null;
            }

            Bitfield bitfield = new Bitfield(data.Length - 20);

            for (int i = 20; i < data.Length; i++)
            {
                bitfield[i - 20] = data[i] == 1;
            }

            return bitfield;
        }

        public void Write(Bitfield bitfield)
        {
            byte[] checksum;
            byte[] data = new byte[20 + bitfield.Length];
            string path = context.Path;

            for (int i = 0; i < bitfield.Length; i++)
            {
                if (bitfield[i])
                {
                    data[i + 20] = 1;
                }
            }

            using (HashAlgorithm algorithm = SHA1.Create())
            {
                checksum = algorithm.ComputeHash(data, 20, data.Length - 20);
            }

            Array.Copy(checksum, 0, data, 0, 20);
            File.WriteAllBytes(path, data);
        }
    }
}