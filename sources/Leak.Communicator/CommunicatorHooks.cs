using System;
using Leak.Events;

namespace Leak.Communicator
{
    public class CommunicatorHooks
    {
        public Action<MessageSent> OnMessageSent;
    }
}