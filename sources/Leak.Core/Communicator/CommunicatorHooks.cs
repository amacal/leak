using System;
using Leak.Events;

namespace Leak.Core.Communicator
{
    public class CommunicatorHooks
    {
        public Action<MessageSent> OnMessageSent;
    }
}