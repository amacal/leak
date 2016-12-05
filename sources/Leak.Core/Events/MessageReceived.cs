using Leak.Common;
using Leak.Core.Network;

namespace Leak.Core.Events
{
    public class MessageReceived
    {
        public PeerHash Peer;

        public string Type;

        public NetworkIncomingMessage Payload;
    }
}