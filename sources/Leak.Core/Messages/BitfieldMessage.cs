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

        public BitfieldMessage(int size)
        {
            this.data = new byte[(size - 1) / 8 + 1];
        }

        public bool this[int index]
        {
            get { return (data[index / 8] & (1 << (byte)(7 - index % 8))) > 0; }
        }

        public int Length
        {
            get { return data.Length + 5; }
        }

        public byte[] ToBytes()
        {
            byte[] result = { 0x00, 0x00, 0x00, 0x00, 0x05 };

            Bytes.Write(data.Length + 1, result, 0);
            Bytes.Append(ref result, data);

            return result;
        }
    }
}