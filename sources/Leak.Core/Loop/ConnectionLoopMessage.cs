using Leak.Core.Messages;
using Leak.Core.Network;

namespace Leak.Core.Loop
{
    public class ConnectionLoopMessage
    {
        private readonly NetworkIncomingMessage incoming;

        public ConnectionLoopMessage(NetworkIncomingMessage incoming)
        {
            this.incoming = incoming;
        }

        public static int GetMessageSize(NetworkIncomingMessage message)
        {
            return message[3] + message[2] * 256 + message[1] * 256 * 256;
        }

        public int Id
        {
            get { return incoming[4]; }
        }

        public int Size
        {
            get { return GetMessageSize(incoming); }
        }

        public byte[] ToBytes()
        {
            return incoming.ToBytes(5, Size - 1);
        }

        public DataBlock ToBlock(DataBlockFactory factory)
        {
            return incoming.ToBlock(factory, 5, Size - 1);
        }
    }
}