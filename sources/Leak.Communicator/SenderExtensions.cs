using Leak.Common;
using Leak.Peer.Sender.Core;
using Leak.Peer.Sender.Events;

namespace Leak.Peer.Sender
{
    public static class SenderExtensions
    {
        public static void CallKeepAliveSent(this SenderHooks hooks, PeerHash peer)
        {
            hooks.OnKeepAliveSent?.Invoke(new KeepAliveSent
            {
                Peer = peer
            });
        }

        public static void CallMessageSent(this SenderHooks hooks, PeerHash peer, string type, SenderMessage message)
        {
            hooks.OnMessageSent?.Invoke(new MessageSent
            {
                Peer = peer,
                Type = type,
                Payload = message
            });
        }

        public static void CallMessageIgnored(this SenderHooks hooks, PeerHash peer, string type, SenderMessage message)
        {
            hooks.OnMessageIgnored?.Invoke(new MessageIgnored
            {
                Peer = peer,
                Type = type,
                Payload = message
            });
        }
    }
}