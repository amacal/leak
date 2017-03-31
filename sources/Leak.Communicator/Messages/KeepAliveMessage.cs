using Leak.Common;

namespace Leak.Communicator.Messages
{
    public class KeepAliveMessage : NetworkOutgoingMessage
    {
        public int Length
        {
            get { return 4; }
        }

        public DataBlock ToBytes(DataBlockFactory factory)
        {
            return factory.Transcient(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, Length);
        }
    }
}