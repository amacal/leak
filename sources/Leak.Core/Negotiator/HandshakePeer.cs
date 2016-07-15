namespace Leak.Core.Negotiator
{
    public class HandshakePeer
    {
        private readonly byte[] value;

        public HandshakePeer(byte[] value)
        {
            this.value = value;
        }

        public byte[] ToBytes()
        {
            return value;
        }
    }
}