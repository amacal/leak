using Leak.Common;

namespace Leak.Networking.Tests
{
    public class OneByteMessage : NetworkOutgoingMessage
    {
        public int Length
        {
            get { return 1; }
        }

        public byte[] ToBytes()
        {
            return new byte[] { 0x00 };
        }
    }
}