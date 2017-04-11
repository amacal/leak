using System;
using Leak.Events;

namespace Leak.Peer.Communicator
{
    public class CommunicatorHooks
    {
        public Action<MessageSent> OnMessageSent;
    }
}