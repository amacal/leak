using Leak.Core.Events;
using System;

namespace Leak.Core.Communicator
{
    public class CommunicatorHooks
    {
        public Action<MessageSent> OnMessageSent;
    }
}