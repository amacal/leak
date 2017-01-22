using Leak.Events;
using System;

namespace Leak.Communicator
{
    public class CommunicatorHooks
    {
        public Action<MessageSent> OnMessageSent;
    }
}