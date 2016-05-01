namespace Leak.Core.Net
{
    public class PeerBitfield
    {
        private readonly byte[] data;
        private readonly int size;

        public PeerBitfield(int size)
        {
            this.size = size;
            this.data = new byte[(size - 1) / 8 + 1];
        }

        public PeerBitfield(PeerMessage message)
        {
            this.size = message.Length * 8;
            this.data = message.ToBytes(5, message.Length - 5);
        }

        public bool this[int index]
        {
            get { return (data[index / 8] & (1 << (byte)(7 - (index % 8)))) > 0; }
            set { data[index / 8] |= (byte)(1 << (byte)(7 - (index % 8))); }
        }

        public void Apply(PeerBitfield bitfield)
        {
            for (int i = 0; i < size; i++)
            {
                if (bitfield[i] == true)
                {
                    this[i] = bitfield[i];
                }
            }
        }

        public void Apply(PeerHave have)
        {
            this[have.Index] = true;
        }
    }
}