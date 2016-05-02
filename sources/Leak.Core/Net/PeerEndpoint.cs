using Leak.Core.IO;

namespace Leak.Core.Net
{
    public class PeerEndpoint
    {
        private readonly TorrentPieceCollection pieces;
        private readonly PeerBitfield bitfield;

        private PeerStatus status;

        public PeerEndpoint(TorrentPieceCollection pieces)
        {
            this.pieces = pieces;
            this.bitfield = new PeerBitfield(pieces.Count);

            this.status = PeerStatus.Choking;
        }

        public PeerBitfield Bitfield
        {
            get { return bitfield; }
        }

        public void Unchoke()
        {
            status &= ~PeerStatus.Choking;
        }

        public bool IsChoking()
        {
            return status.HasFlag(PeerStatus.Choking);
        }
    }
}