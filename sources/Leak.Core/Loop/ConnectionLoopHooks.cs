using Leak.Core.Events;
using System;

namespace Leak.Core.Loop
{
    public class ConnectionLoopHooks
    {
        public Action<MessageReceived> OnMessageReceived;
    }
}