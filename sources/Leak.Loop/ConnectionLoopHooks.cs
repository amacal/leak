using System;
using Leak.Events;

namespace Leak.Peer.Receiver
{
    public class ConnectionLoopHooks
    {
        public Action<MessageReceived> OnMessageReceived;
    }
}