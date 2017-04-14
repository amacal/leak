using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Coordinator.Messages
{
    public class BitfieldIncomingMessage
    {
        private readonly NetworkIncomingMessage inner;

        public BitfieldIncomingMessage(NetworkIncomingMessage inner)
        {
            this.inner = inner;
        }

        public bool this[int index]
        {
            get { return (inner[5 + index / 8] & (1 << (byte)(7 - index % 8))) > 0; }
        }

        public int Size
        {
            get { return inner.Length * 8 - 40; }
        }

        public Bitfield ToBitfield()
        {
            Bitfield bitfield = new Bitfield(Size);

            for (int i = 0; i < Size; i++)
            {
                bitfield[i] = this[i];
            }

            return bitfield;
        }
    }
}