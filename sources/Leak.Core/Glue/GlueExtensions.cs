using Leak.Core.Common;
using Leak.Core.Events;

namespace Leak.Core.Glue
{
    public static class GlueExtensions
    {
        public static void CallPeerConnected(this GlueHooks hooks, PeerHash peer)
        {
            hooks.OnPeerConnected?.Invoke(new PeerConnected
            {
                Peer = peer
            });
        }

        public static void CallPeerDisconnected(this GlueHooks hooks, PeerHash peer)
        {
            hooks.OnPeerDisconnected?.Invoke(new PeerDisconnected
            {
                Peer = peer
            });
        }

        public static void CallPeerBitfieldChanged(this GlueHooks hooks, PeerHash peer, Bitfield bitfield)
        {
            hooks.OnPeerBitfieldChanged?.Invoke(new PeerBitfieldChanged
            {
                Peer = peer
            });
        }

        public static void CallPeerStateChanged(this GlueHooks hooks, PeerHash peer, GlueState state)
        {
            hooks.OnPeerBitfieldChanged?.Invoke(new PeerBitfieldChanged
            {
                Peer = peer
            });
        }
    }
}