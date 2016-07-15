namespace Leak.Core.Negotiator
{
    public class HandshakeHash
    {
        private readonly byte[] value;

        public HandshakeHash(byte[] value)
        {
            this.value = value;
        }

        public byte[] ToBytes()
        {
            return value;
        }
    }
}