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

        public static int GetInt32(this NetworkIncomingMessage incoming, int offset)
        {
            int value = 0;

            for (int i = 0; i < 4; i++)
            {
                value = (value << 8) + incoming[offset + i + 5];
            }

            return value;
        }

        public static byte[] GetBytes(this NetworkIncomingMessage incoming)
        {
            return incoming.ToBytes(5, incoming.GetSize() - 1);
        }

        public static DataBlock GetBlock(this NetworkIncomingMessage incoming, DataBlockFactory factory)
        {
            return incoming.ToBlock(factory, 5, incoming.GetSize() - 1);
        }

        public static Bitfield GetBitfield(this NetworkIncomingMessage incoming)
        {
            return new BitfieldIncomingMessage(incoming.ToBytes()).ToBitfield();
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

        public static void CallOnPeerKeepAliveMessageReceived(this ConnectionLoopHooks hooks, PeerHash peer)
        {
            hooks?.OnPeerKeepAliveMessageReceived?.Invoke(new PeerKeepAliveMessageReceived
            {
                Peer = peer
            });
        }

        public static void CallOnPeerChokeMessageReceived(this ConnectionLoopHooks hooks, PeerHash peer)
        {
            hooks?.OnPeerChokeMessageReceived?.Invoke(new PeerChokeMessageReceived
            {
                Peer = peer
            });
        }

        public static void CallOnPeerUnchokeMessageReceived(this ConnectionLoopHooks hooks, PeerHash peer)
        {
            hooks?.OnPeerUnchokeMessageReceived?.Invoke(new PeerUnchokeMessageReceived
            {
                Peer = peer
            });
        }

        public static void CallOnPeerInterestedMessageReceived(this ConnectionLoopHooks hooks, PeerHash peer)
        {
            hooks?.OnPeerInterestedMessageReceived?.Invoke(new PeerInterestedMessageReceived
            {
                Peer = peer
            });
        }

        public static void CallOnPeerHaveMessageReceived(this ConnectionLoopHooks hooks, PeerHash peer, NetworkIncomingMessage message)
        {
            hooks?.OnPeerHaveMessageReceived?.Invoke(new PeerHaveMessageReceived
            {
                Peer = peer,
                Piece = message.GetInt32(0)
            });
        }

        public static void CallOnPeerBitfieldMessageReceived(this ConnectionLoopHooks hooks, PeerHash peer, NetworkIncomingMessage message)
        {
            hooks?.OnPeerBitfieldMessageReceived?.Invoke(new PeerBitfieldMessageReceived
            {
                Peer = peer,
                Bitfield = message.GetBitfield()
            });
        }

        public static void CallOnPeerPieceMessageReceived(this ConnectionLoopHooks hooks, PeerHash peer, NetworkIncomingMessage message, DataBlockFactory factory)
        {
            hooks?.OnPeerPieceMessageReceived?.Invoke(new PeerPieceMessageReceived
            {
                Peer = peer,
                Data = message.GetBlock(factory)
            });
        }

        public static void CallOnPeerExtendedMessageReceived(this ConnectionLoopHooks hooks, PeerHash peer, NetworkIncomingMessage message)
        {
            hooks?.OnPeerExtendedMessageReceived?.Invoke(new PeerExtendedMessageReceived
            {
                Peer = peer
            });
        }
    }
}