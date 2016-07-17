namespace Leak.Core.Listener
{
    public class PeerListenerPeer
    {
        private readonly byte[] value;

        public PeerListenerPeer(byte[] value)
        {
            this.value = value;
        }

        public byte[] ToBytes()
        {
            return value;
        }
    }
}