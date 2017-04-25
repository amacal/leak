using Leak.Bencoding;
using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Coordinator.Core;
using Leak.Peer.Coordinator.Events;
using Leak.Peer.Coordinator.Messages;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Coordinator
{
    public static class CoordinatorExtensions
    {
        public static Bitfield GetBitfield(this NetworkIncomingMessage incoming)
        {
            return new BitfieldIncomingMessage(incoming).ToBitfield();
        }

        public static Piece GetPiece(this NetworkIncomingMessage incoming, DataBlockFactory factory)
        {
            return new PieceIncomingMessage(incoming).ToPiece(factory);
        }

        public static Request GetRequest(this NetworkIncomingMessage incoming)
        {
            return new RequestIncomingMessage(incoming).ToRequest();
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

        public static void SendKeepAlive(this CoordinatorHooks hooks, PeerHash peer)
        {
            hooks.CallKeeAliveRequested(peer);
        }

        public static void SendBitfield(this CoordinatorHooks hooks, PeerHash peer, Bitfield bitfield)
        {
            hooks.CallMessageRequested(peer, new CoordinatorBitfieldMessage(bitfield));
        }

        public static void SendHave(this CoordinatorHooks hooks, PeerHash peer, PieceInfo piece)
        {
            hooks.CallMessageRequested(peer, new CoordinatorHaveMessage(piece));
        }

        public static void SendRequest(this CoordinatorHooks hooks, PeerHash peer, Request request)
        {
            hooks.CallMessageRequested(peer, new CoordinatorRequestMessage(request));
        }

        public static void SendPiece(this CoordinatorHooks hooks, PeerHash peer, Piece piece)
        {
            hooks.CallMessageRequested(peer, new CoordinatorPieceMessage(piece));
        }

        public static void SendExtended(this CoordinatorHooks hooks, PeerHash peer, Extended extended)
        {
            hooks.CallMessageRequested(peer, new CoordinatorExtendedMessage(extended));
        }

        public static void SendChoke(this CoordinatorHooks hooks, PeerHash peer, bool value)
        {
            string type = value ? "choke" : "unchoke";
            SenderMessage message = new CoordinatorGenericMessage(type);

            hooks.CallMessageRequested(peer, message);
        }

        public static void SendInterested(this CoordinatorHooks hooks, PeerHash peer)
        {
            hooks.CallMessageRequested(peer, new CoordinatorGenericMessage("interested"));
        }

        public static void CallPeerConnected(this CoordinatorHooks hooks, PeerHash peer, NetworkConnection connection)
        {
            hooks.OnPeerConnected?.Invoke(new PeerConnected
            {
                Peer = peer,
                Connection = connection
            });
        }

        public static void CallKeeAliveRequested(this CoordinatorHooks hooks, PeerHash peer)
        {
            hooks.OnKeepAliveRequested?.Invoke(new KeepAliveRequested
            {
                Peer = peer
            });
        }

        public static void CallMessageRequested(this CoordinatorHooks hooks, PeerHash peer, SenderMessage message)
        {
            hooks.OnMessageRequested?.Invoke(new MessageRequested
            {
                Peer = peer,
                Message = message
            });
        }

        public static void CallPeerDisconnected(this CoordinatorHooks hooks, PeerHash peer, NetworkAddress remote)
        {
            hooks.OnPeerDisconnected?.Invoke(new PeerDisconnected
            {
                Peer = peer,
                Remote = remote
            });
        }

        public static void CallBitfieldChanged(this CoordinatorHooks hooks, PeerHash peer, Bitfield bitfield)
        {
            hooks.OnBitfieldChanged?.Invoke(new BitfieldChanged
            {
                Peer = peer,
                Bitfield = bitfield,
            });
        }

        public static void CallBitfieldChanged(this CoordinatorHooks hooks, PeerHash peer, Bitfield bitfield, PieceInfo affected)
        {
            hooks.OnBitfieldChanged?.Invoke(new BitfieldChanged
            {
                Peer = peer,
                Bitfield = bitfield,
                Affected = affected
            });
        }

        public static void CallBlockReceived(this CoordinatorHooks hooks, FileHash hash, PeerHash peer, Piece piece)
        {
            hooks.OnBlockReceived?.Invoke(new BlockReceived
            {
                Hash = hash,
                Peer = peer,
                Block = piece.Index,
                Payload = piece.Data
            });
        }

        public static void CallBlockRequested(this CoordinatorHooks hooks, FileHash hash, PeerHash peer, Request request)
        {
            hooks.OnBlockRequested?.Invoke(new BlockRequested
            {
                Hash = hash,
                Peer = peer,
                Block = request.Block
            });
        }

        public static void CallStatusChanged(this CoordinatorHooks hooks, PeerHash peer, PeerState state)
        {
            hooks.OnStatusChanged?.Invoke(new StatusChanged
            {
                Peer = peer,
                State = state,
            });
        }

        public static void CallExtensionListReceived(this CoordinatorHooks hooks, PeerHash peer, string[] extensions)
        {
            hooks.OnExtensionListReceived?.Invoke(new ExtensionListReceived
            {
                Peer = peer,
                Extensions = extensions
            });
        }

        public static void CallExtensionListSent(this CoordinatorHooks hooks, PeerHash peer, string[] extensions)
        {
            hooks.OnExtensionListSent?.Invoke(new ExtensionListSent
            {
                Peer = peer,
                Extensions = extensions
            });
        }

        public static void CallExtensionDataReceived(this CoordinatorHooks hooks, PeerHash peer, string extension, int size)
        {
            hooks.OnExtensionDataReceived?.Invoke(new ExtensionDataReceived
            {
                Peer = peer,
                Extension = extension,
                Size = size
            });
        }

        public static void CallExtensionDataSent(this CoordinatorHooks hooks, PeerHash peer, string extension, int size)
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