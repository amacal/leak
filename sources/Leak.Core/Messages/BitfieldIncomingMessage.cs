using Leak.Common;
using Leak.Core.Network;

namespace Leak.Core.Messages
{
    public class BitfieldIncomingMessage
    {
        private readonly NetworkIncomingMessage message;

        public BitfieldIncomingMessage(NetworkIncomingMessage message)
        {
            this.message = message;
        }

        public bool this[int index]
        {
            get { return (message[5 + index / 8] & (1 << (byte)(7 - index % 8))) > 0; }
        }

        public int Size
        {
            get { return message.Length * 8 - 40; }
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