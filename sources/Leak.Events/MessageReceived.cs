using Leak.Common;

namespace Leak.Events
{
    public class MessageReceived
    {
        public PeerHash Peer;

        public string Type;

        public NetworkIncomingMessage Payload;
    }
}