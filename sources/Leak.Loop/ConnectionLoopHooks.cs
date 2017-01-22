using Leak.Events;
using System;

namespace Leak.Loop
{
    public class ConnectionLoopHooks
    {
        public Action<MessageReceived> OnMessageReceived;
    }
}