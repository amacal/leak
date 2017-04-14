namespace Leak.Peer.Coordinator.Core
{
    public class Extended
    {
        private readonly byte id;
        private readonly byte[] data;

        public Extended(byte id, byte[] data)
        {
            this.id = id;
            this.data = data;
        }

        public byte Id
        {
            get { return id; }
        }

        public byte[] Data
        {
            get { return data; }
        }
    }
}