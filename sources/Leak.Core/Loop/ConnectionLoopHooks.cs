using System;
using Leak.Events;

namespace Leak.Core.Loop
{
    public class ConnectionLoopHooks
    {
        public Action<MessageReceived> OnMessageReceived;
    }
}