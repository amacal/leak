using System;
using Leak.Events;

namespace Leak.Loop
{
    public class ConnectionLoopHooks
    {
        public Action<MessageReceived> OnMessageReceived;
    }
}