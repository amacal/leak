using Leak.Core.Network;

namespace Leak.Core.Messages
{
    public class BitfieldMessage : NetworkOutgoingMessage
    {
        private readonly byte[] data;

        public BitfieldMessage(byte[] data)
        {
            this.data = data;
        }

        public bool this[int index]
        {
            get { return (data[index / 8] & (1 << (byte)(7 - index % 8))) > 0; }
        }

        public int Length
        {
            get { return data.Length; }
        }

        public byte[] ToBytes()
        {
            return data;
        }
    }
}