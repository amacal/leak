using Leak.Events;
using System;

namespace Leak.Core.Loop
{
    public class ConnectionLoopHooks
    {
        public Action<MessageReceived> OnMessageReceived;
    }
}