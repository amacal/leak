namespace Leak.Core.Listener
{
    public class PeerListenerHash
    {
        private readonly byte[] value;

        public PeerListenerHash(byte[] value)
        {
            this.value = value;
        }

        public byte[] ToBytes()
        {
            return value;
        }
    }
}