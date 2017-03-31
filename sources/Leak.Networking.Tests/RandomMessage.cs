using Leak.Common;

namespace Leak.Networking.Tests
{
    public class RandomMessage : NetworkOutgoingMessage
    {
        private readonly int length;

        public RandomMessage(int length)
        {
            this.length = length;
        }

        public int Length
        {
            get { return length; }
        }

        public DataBlock ToBytes(DataBlockFactory factory)
        {
            return factory.Transcient(Bytes.Random(length), 0, length);
        }
    }
}