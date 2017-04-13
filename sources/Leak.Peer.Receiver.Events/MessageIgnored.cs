using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Receiver.Events
{
    public class MessageIgnored
    {
        public byte Identifier;

        public PeerHash Peer;

        public NetworkIncomingMessage Payload;
    }
}