using Leak.Common;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Sender.Events
{
    public class MessageSent
    {
        public PeerHash Peer;

        public string Type;

        public SenderMessage Payload;
    }
}