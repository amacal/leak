using System.Linq;

namespace Leak.Core.Negotiator
{
    public class HandshakeRivestCipher
    {
        private readonly byte[] key;
        private readonly byte[] vector;

        private long position;
        private int i, j;

        public HandshakeRivestCipher(byte[] key, int discard)
        {
            this.key = key;
            this.vector = Initialize(key);
            this.Skip(discard);
        }

        private HandshakeRivestCipher(HandshakeRivestCipher source)
        {
            this.key = source.key.ToArray();
            this.vector = source.vector.ToArray();
            this.position = source.position;
            this.i = source.i;
            this.j = source.j;
        }

        public void Skip(int count)
        {
            Handle(new byte[count]);
        }

        public byte[] Encrypt(byte[] data)
        {
            return Handle(data);
        }

        public byte[] Decrypt(byte[] data)
        {
            return Handle(data);
        }

        public HandshakeRivestCipher Clone()
        {
            return new HandshakeRivestCipher(this);
        }

        private static byte[] Initialize(byte[] key)
        {
            byte[] s = Enumerable.Range(0, 256)
              .Select(i => (byte)i)
              .ToArray();

            for (int i = 0, j = 0; i < 256; i++)
            {
                j = (j + key[i % key.Length] + s[i]) & 255;

                Swap(s, i, j);
            }

            return s;
        }

        private byte[] Handle(byte[] data)
        {
            int length = data.Length;
            byte[] result = new byte[length];

            for (int x = 0; x < length; x++)
            {
                i = (i + 1) & 255;
                j = (j + vector[i]) & 255;

                Swap(vector, i, j);
                position++;

                result[x] = (byte)(data[x] ^ vector[(vector[i] + vector[j]) & 255]);
            }

            return result;
        }

        private static void Swap(byte[] s, int i, int j)
        {
            byte c = s[i];

            s[i] = s[j];
            s[j] = c;
        }

        public override string ToString()
        {
            return $"position: {position}";
        }
    }
}