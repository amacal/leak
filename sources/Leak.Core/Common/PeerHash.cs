namespace Leak.Core.Common
{
    public class PeerHash
    {
        private readonly byte[] value;

        public PeerHash(byte[] value)
        {
            this.value = value;
        }

        public byte[] ToBytes()
        {
            return value;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            PeerHash other = obj as PeerHash;

            return other != null && Bytes.Equals(other.value, value);
        }

        public override string ToString()
        {
            return Bytes.ToString(value);
        }

        public static PeerHash Random()
        {
            return new PeerHash(Bytes.Random(20));
        }

        public static PeerHash Random(string prefix)
        {
            byte[] value = Bytes.Parse(prefix);
            byte[] random = Bytes.Random(20 - value.Length);

            Bytes.Append(ref value, random);
            return new PeerHash(value);
        }
    }
}