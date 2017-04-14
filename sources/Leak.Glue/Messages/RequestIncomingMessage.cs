using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Coordinator.Core;

namespace Leak.Peer.Coordinator.Messages
{
    public class RequestIncomingMessage
    {
        private readonly NetworkIncomingMessage inner;

        public RequestIncomingMessage(NetworkIncomingMessage inner)
        {
            this.inner = inner;
        }

        public Request ToRequest()
        {
            int piece = inner[8] + inner[7] * 256 + inner[6] * 256 * 256;
            int offset = inner[12] + inner[11] * 256 + inner[10] * 256 * 256;
            int length = inner[16] + inner[15] * 256 + inner[14] * 256 * 256;

            return new Request(new BlockIndex(piece, offset, length));
        }
    }
}