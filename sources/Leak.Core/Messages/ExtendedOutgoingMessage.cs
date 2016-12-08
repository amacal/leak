using Leak.Common;

namespace Leak.Core.Messages
{
    public class ExtendedOutgoingMessage : NetworkOutgoingMessage
    {
        private readonly Extended extended;

        public ExtendedOutgoingMessage(Extended extended)
        {
            this.extended = extended;
        }

        public int Length
        {
            get { return 6 + extended.Data.Length; }
        }

        public byte[] ToBytes()
        {
            byte[] bytes = { 0x00, 0x00, 0x00, 0x01, 0x14, extended.Id };

            Bytes.Write(extended.Data.Length + 2, bytes, 0);
            Bytes.Append(ref bytes, extended.Data);

            return bytes;
        }
    }
}