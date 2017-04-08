using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Events
{
    public class MessageReceived
    {
        public PeerHash Peer;

        public string Type;

        public NetworkIncomingMessage Payload;
    }
}