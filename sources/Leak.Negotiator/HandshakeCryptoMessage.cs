using Leak.Common;

namespace Leak.Negotiator
{
    public class HandshakeCryptoMessage : NetworkOutgoingMessage
    {
        public static readonly int MinimumSize = 2;

        public static int GetSize(NetworkIncomingMessage message)
        {
            return 2 + message[0] * 256 + message[1];
        }

        public int Length
        {
            get { return 2; }
        }

        public DataBlock ToBytes(DataBlockFactory factory)
        {
            return factory.Transcient(Bytes.Parse("0000"), 0, Length);
        }
    }
}