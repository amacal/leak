using Leak.Common;

namespace Leak.Communicator.Messages
{
    public class ChokeMessage : NetworkOutgoingMessage
    {
        public int Length
        {
            get { return 5; }
        }

        public byte[] ToBytes()
        {
            return new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00 };
        }
    }
}