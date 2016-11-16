using Leak.Core.Common;
using Leak.Core.Events;
using Leak.Core.Messages;
using Leak.Core.Network;

namespace Leak.Core.Glue
{
    public static class GlueExtensions
    {
        public static Bitfield GetBitfield(this NetworkIncomingMessage incoming)
        {
            return new BitfieldIncomingMessage(incoming).ToBitfield();
        }

        public static int GetInt32(this NetworkIncomingMessage incoming, int offset)
        {
            int value = 0;

            for (int i = 0; i < 4; i++)
            {
                value = (value << 8) + incoming[offset + i + 5];
            }

            return value;
        }

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
                Peer = peer,
                Bitfield = bitfield
            });
        }

        public static void CallPeerStateChanged(this GlueHooks hooks, PeerHash peer, GlueState state)
        {
            hooks.OnPeerStateChanged?.Invoke(new PeerStateChanged
            {
                Peer = peer,
                IsLocalChokingRemote = state.HasFlag(GlueState.IsLocalChockingRemote),
                IsLocalInterestedInRemote = state.HasFlag(GlueState.IsLocalInterestedInRemote),
                IsRemoteChokingLocal = state.HasFlag(GlueState.IsRemoteChockingLocal),
                IsRemoteInterestedInLocal = state.HasFlag(GlueState.IsRemoteInterestedInLocal)
            });
        }
    }
}