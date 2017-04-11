using System.Linq;

namespace Leak.Peer.Negotiator
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

        public byte[] Encrypt(byte[] data, int index, int count)
        {
            return Handle(data, true, index, count);
        }

        public byte[] Decrypt(byte[] data)
        {
            return Handle(data);
        }

        public void Decrypt(byte[] data, int index, int count)
        {
            Handle(data, true, index, count);
        }

        public HandshakeRivestCipher Clone()
        {
            return new HandshakeRivestCipher(this);
        }

        private static byte[] Initialize(byte[] key)
        {
            byte c;
            byte[] s = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                s[i] = (byte)i;
            }

            for (int i = 0, j = 0; i < 256; i++)
            {
                j = (j + key[i % key.Length] + s[i]) & 255;

                c = s[i];
                s[i] = s[j];
                s[j] = c;
            }

            return s;
        }

        private byte[] Handle(byte[] data)
        {
            return Handle(data, false, 0, data.Length);
        }

        private byte[] Handle(byte[] data, bool inPlace, int index, int count)
        {
            byte c;
            int length = index + count;
            byte[] result = inPlace ? data : new byte[length];

            for (int x = index; x < length; x++)
            {
                i = (i + 1) & 255;
                j = (j + vector[i]) & 255;

                c = vector[i];
                vector[i] = vector[j];
                vector[j] = c;

                position++;
                result[x] = (byte)(data[x] ^ vector[(vector[i] + vector[j]) & 255]);
            }

            return result;
        }

        public override string ToString()
        {
            return $"position: {position}";
        }
    }
}