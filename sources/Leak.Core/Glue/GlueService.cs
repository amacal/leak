using Leak.Core.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;

namespace Leak.Core.Glue
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

        void SendHave(PeerHash peer, int piece);

        void SendExtension(PeerHash peer, string extension, byte[] payload);

        bool IsSupported(PeerHash peer, string extension);
    }
}