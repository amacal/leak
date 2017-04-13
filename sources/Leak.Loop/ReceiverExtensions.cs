using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Receiver.Events;

namespace Leak.Peer.Receiver
{
    public static class ReceiverExtensions
    {
        public static int GetSize(this NetworkIncomingMessage incoming)
        {
            return incoming[3] + incoming[2] * 256 + incoming[1] * 256 * 256;
        }

        public static byte[] GetBytes(this NetworkIncomingMessage incoming)
        {
            return incoming.ToBytes(5, incoming.GetSize() - 1);
        }

        public static DataBlock GetBlock(this NetworkIncomingMessage incoming, DataBlockFactory factory)
        {
            return incoming.ToBlock(factory, 5, incoming.GetSize() - 1);
        }

        public static NetworkIncomingMessage Restricted(this NetworkIncomingMessage incoming)
        {
            return incoming.Restricted(incoming.GetSize() + 4);
        }

        public static void CallKeepAliveReceived(this ReceiverHooks hooks, PeerHash peer)
        {
            hooks.OnKeepAliveReceived?.Invoke(new KeepAliveReceived
            {
                Peer = peer
            });
        }

        public static void CallMessageReceived(this ReceiverHooks hooks, PeerHash peer, string type, NetworkIncomingMessage payload)
        {
            hooks.OnMessageReceived?.Invoke(new MessageReceived
            {
                Peer = peer,
                Type = type,
                Payload = payload
            });
        }

        public static void CallMessageIgnored(this ReceiverHooks hooks, PeerHash peer, byte identifier, NetworkIncomingMessage payload)
        {
            hooks.OnMessageIgnored?.Invoke(new MessageIgnored
            {
                Peer = peer,
                Identifier = identifier,
                Payload = payload
            });
        }
    }
}