using Leak.Core.Bencoding;
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

        public static bool IsHandshake(this NetworkIncomingMessage incoming)
        {
            return incoming[5] == 0;
        }

        public static BencodedValue GetBencoded(this NetworkIncomingMessage incoming)
        {
            byte[] binary = incoming.ToBytes(6);
            BencodedValue bencoded = Bencoder.Decode(binary);

            return bencoded;
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

        public static void CallExtensionListReceived(this GlueHooks hooks, PeerHash peer, string[] extensions)
        {
            hooks.OnExtensionListReceived?.Invoke(new ExtensionListReceived
            {
                Peer = peer,
                Extensions = extensions
            });
        }

        public static void CallExtensionListSent(this GlueHooks hooks, PeerHash peer, string[] extensions)
        {
            hooks.OnExtensionListSent?.Invoke(new ExtensionListSent
            {
                Peer = peer,
                Extensions = extensions
            });
        }
    }
}