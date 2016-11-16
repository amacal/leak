using Leak.Core.Common;

namespace Leak.Core.Messages
{
    public class BitfieldIncomingMessage
    {
        private readonly byte[] data;

        public BitfieldIncomingMessage(byte[] data)
        {
            this.data = data;
        }

        public bool this[int index]
        {
            get { return (data[index / 8] & (1 << (byte)(7 - index % 8))) > 0; }
        }

        public int Size
        {
            get { return data.Length * 8; }
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