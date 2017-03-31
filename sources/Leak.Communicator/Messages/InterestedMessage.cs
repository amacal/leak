using Leak.Common;

namespace Leak.Communicator.Messages
{
    public class InterestedMessage : NetworkOutgoingMessage
    {
        public int Length
        {
            get { return 5; }
        }

        public DataBlock ToBytes(DataBlockFactory factory)
        {
            return factory.Transcient(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02 }, 0, Length);
        }
    }
}