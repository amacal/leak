using System;
using Leak.Common;

namespace Leak.Glue
{
    public interface GlueService
    {
        FileHash Hash { get; }

        bool Connect(NetworkConnection connection, Handshake handshake);

        bool Disconnect(NetworkConnection connection);

        void SetPieces(int pieces);

        void SendChoke(PeerHash peer);

        void SendUnchoke(PeerHash peer);

        void SendInterested(PeerHash peer);

        void SendBitfield(PeerHash peer, Bitfield bitfield);

        void SendRequest(PeerHash peer, int piece, int offset, int size);

        void SendHave(PeerHash peer, int piece);

        void SendExtension(PeerHash peer, string extension, byte[] payload);

        bool IsSupported(PeerHash peer, string extension);

        void ForEachPeer(Action<PeerHash> callback);

        void ForEachPeer(Action<PeerHash, PeerAddress> callback);

        void ForEachPeer(Action<PeerHash, PeerAddress, NetworkDirection> callback);
    }
}