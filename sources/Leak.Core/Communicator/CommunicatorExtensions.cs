using Leak.Common;
using Leak.Core.Events;
using Leak.Core.Network;

namespace Leak.Core.Communicator
{
    public static class CommunicatorExtensions
    {
        public static void CallMessageSent(this CommunicatorHooks hooks, PeerHash peer, string type, NetworkOutgoingMessage payload)
        {
            hooks.OnMessageSent?.Invoke(new MessageSent
            {
                Peer = peer,
                Type = type,
                Payload = payload
            });
        }
    }
}