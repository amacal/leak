using Leak.Core.Common;
using Leak.Core.Network;

namespace Leak.Core.Events
{
    public class MessageSent
    {
        public PeerHash Peer;

        public string Type;

        public NetworkOutgoingMessage Payload;
    }
}