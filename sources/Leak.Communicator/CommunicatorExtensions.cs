using Leak.Common;
using Leak.Events;
using Leak.Networking.Core;

namespace Leak.Peer.Communicator
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