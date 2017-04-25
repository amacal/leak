using Leak.Common;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Coordinator.Events
{
    public class MessageRequested
    {
        public PeerHash Peer;
        public SenderMessage Message;
    }
}