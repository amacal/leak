using Leak.Networking.Core;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Sender.Tests
{
    public class SenderUnknown : SenderOutgoingMessage, NetworkOutgoingMessage
    {
        public string Type
        {
            get { return "unknown"; }
        }

        public NetworkOutgoingMessage Apply(byte id)
        {
            return this;
        }

        public int Length
        {
            get { return 5; }
        }

        public void ToBytes(DataBlock block)
        {
        }

        public void Release()
        {
        }
    }
}