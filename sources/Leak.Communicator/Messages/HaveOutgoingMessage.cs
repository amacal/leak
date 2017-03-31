using Leak.Common;

namespace Leak.Communicator.Messages
{
    public class HaveOutgoingMessage : NetworkOutgoingMessage
    {
        private readonly int piece;

        public HaveOutgoingMessage(int piece)
        {
            this.piece = piece;
        }

        public int Length
        {
            get { return 9; }
        }

        public DataBlock ToBytes(DataBlockFactory factory)
        {
            byte lowest = (byte)(piece % 256);
            byte highest = (byte)(piece / 256);

            return factory.Transcient(new byte[] { 0x00, 0x00, 0x00, 0x05, 0x04, 0x00, 0x00, highest, lowest }, 0, Length);
        }
    }
}