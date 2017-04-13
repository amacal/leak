using System;
using Leak.Peer.Receiver.Events;

namespace Leak.Peer.Receiver
{
    public class ReceiverHooks
    {
        public Action<KeepAliveReceived> OnKeepAliveReceived;

        public Action<MessageReceived> OnMessageReceived;

        public Action<MessageIgnored> OnMessageIgnored;
    }
}