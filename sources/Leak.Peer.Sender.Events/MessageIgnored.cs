using Leak.Common;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Sender.Events
{
    public class MessageIgnored
    {
        public string Type;

        public PeerHash Peer;

        public SenderMessage Payload;
    }
}