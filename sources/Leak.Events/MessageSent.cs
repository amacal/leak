using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Events
{
    public class MessageSent
    {
        public PeerHash Peer;

        public string Type;

        public NetworkOutgoingMessage Payload;
    }
}