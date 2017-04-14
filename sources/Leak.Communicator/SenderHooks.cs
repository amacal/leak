using System;
using Leak.Peer.Sender.Events;

namespace Leak.Peer.Sender
{
    public class SenderHooks
    {
        public Action<KeepAliveSent> OnKeepAliveSent;

        public Action<MessageSent> OnMessageSent;

        public Action<MessageIgnored> OnMessageIgnored;
    }
}