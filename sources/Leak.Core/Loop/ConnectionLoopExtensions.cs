using Leak.Core.Common;
using Leak.Core.Events;
using Leak.Core.Messages;
using Leak.Core.Network;

namespace Leak.Core.Loop
{
    public static class ConnectionLoopExtensions
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

        public static void CallMessageReceived(this ConnectionLoopHooks hooks, PeerHash peer, string type, NetworkIncomingMessage payload)
        {
            hooks.OnMessageReceived?.Invoke(new MessageReceived
            {
                Peer = peer,
                Type = type,
                Payload = payload
            });
        }
    }
}