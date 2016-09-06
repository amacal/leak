using Leak.Core.Common;
using Leak.Core.Omnibus.Events;

namespace Leak.Core.Omnibus
{
    public interface OmnibusCallback
    {
        /// <summary>
        /// Called when the hash is completed.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnFileCompleted(FileHash hash);

        /// <summary>
        /// Called when the progress changed.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="bitfield">The current bitfield describing the progress.</param>
        void OnProgressChanged(FileHash hash, BitfieldInfo bitfield);

        /// <summary>
        /// Called when the piece has completed all blocks but it is not verified yet.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="piece">The index of the affected piece.</param>
        void OnPieceReady(FileHash hash, PieceInfo piece);

        /// <summary>
        /// Called when the piece has completed and verified all blocks.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="piece">The index of the affected piece.</param>
        void OnPieceCompleted(FileHash hash, PieceInfo piece);

        /// <summary>
        /// Called when the blocks were reserved for the requesting peer.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="event">The reservation details.</param>
        void OnBlockReserved(FileHash hash, OmnibusReservationEvent @event);

        /// <summary>
        /// Called when the block expired, because the peer didn't completed
        /// in the requested time frame.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="peer">The hash of the affected peer.</param>
        /// <param name="block">The affected block.</param>
        void OnBlockExpired(FileHash hash, PeerHash peer, OmnibusBlock block);
    }
}