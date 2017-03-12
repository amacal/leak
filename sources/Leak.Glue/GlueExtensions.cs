using Leak.Bencoding;
using Leak.Common;
using Leak.Communicator.Messages;
using Leak.Events;

namespace Leak.Glue
{
    public static class GlueExtensions
    {
        public static Bitfield GetBitfield(this NetworkIncomingMessage incoming)
        {
            return new BitfieldIncomingMessage(incoming).ToBitfield();
        }

        public static Piece GetPiece(this NetworkIncomingMessage incoming, DataBlockFactory factory)
        {
            return new PieceIncomingMessage(incoming).ToPiece(factory);
        }

        public static bool IsExtensionHandshake(this NetworkIncomingMessage incoming)
        {
            return incoming[5] == 0;
        }

        public static byte GetExtensionIdentifier(this NetworkIncomingMessage incoming)
        {
            return incoming[5];
        }

        public static int GetExtensionSize(this NetworkIncomingMessage incoming)
        {
            return incoming.Length - 6;
        }

        public static byte[] GetExtensionData(this NetworkIncomingMessage incoming)
        {
            return incoming.ToBytes(6);
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
                Bitfield = bitfield,
            });
        }

        public static void CallBlockReceived(this GlueHooks hooks, FileHash hash, PeerHash peer, Piece piece)
        {
            hooks.OnBlockReceived?.Invoke(new BlockReceived
            {
                Hash = hash,
                Peer = peer,
                Block = piece.Index,
                Payload = piece.Data
            });
        }

        public static void CallPeerStatusChanged(this GlueHooks hooks, PeerHash peer, PeerState state)
        {
            hooks.OnPeerStatusChanged?.Invoke(new PeerStatusChanged
            {
                Peer = peer,
                State = state,
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

        public static void CallExtensionDataReceived(this GlueHooks hooks, PeerHash peer, string extension, int size)
        {
            hooks.OnExtensionDataReceived?.Invoke(new ExtensionDataReceived
            {
                Peer = peer,
                Extension = extension,
                Size = size
            });
        }

        public static void CallExtensionDataSent(this GlueHooks hooks, PeerHash peer, string extension, int size)
        {
            hooks.OnExtensionDataSent?.Invoke(new ExtensionDataSent
            {
                Peer = peer,
                Extension = extension,
                Size = size
            });
        }
    }
}