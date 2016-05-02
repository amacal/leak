using System;
using System.Numerics;

namespace Leak.Core.Net
{
    public static class PeerCryptography
    {
        private static readonly BigInteger Prime;

        static PeerCryptography()
        {
            Prime = new BigInteger(ToBytes("ffffffffffffffffc90fdaa22168c234c4c6628b80dc1cd129024e088a67cc74020bbea63b139b22514a08798e3404ddef9519b3cd3a431b302b0a6df25f14374fe1356d6d51c245e485b576625e7ec6f44c42e9a63a36210000000000090563"));
        }

        public static PeerCredentials Generate()
        {
            BigInteger generator = new BigInteger(2);
            BigInteger privateKey = new BigInteger(Randomize(20));

            if (privateKey.Sign == -1)
            {
                privateKey = -privateKey;
            }

            BigInteger publicKey = BigInteger.ModPow(generator, privateKey, Prime);

            return new PeerCredentials
            {
                PrivateKey = privateKey.ToByteArray(),
                PublicKey = publicKey.ToByteArray(),
                Padding = Randomize(256)
            };
        }

        public static byte[] Secret(PeerCredentials credentials, byte[] publicKey)
        {
            BigInteger xa = new BigInteger(credentials.PrivateKey);
            BigInteger yb = new BigInteger(publicKey);
            BigInteger secret = BigInteger.ModPow(yb, xa, Prime);

            return secret.ToByteArray();
        }

        private static byte[] Randomize(int size)
        {
            byte[] data = new byte[size];
            Random random = new Random();

            random.NextBytes(data);
            return data;
        }

        private static byte[] ToBytes(string value)
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