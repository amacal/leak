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
    }
}