using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Receiver.Events
{
    public class MessageReceived
    {
        public string Type;

        public PeerHash Peer;

        public NetworkIncomingMessage Payload;
    }
}