using Leak.Core.Common;
using Leak.Core.Omnibus.Events;

namespace Leak.Core.Omnibus
{
    public interface OmnibusCallback
    {
        void OnFileCompleted(FileHash hash);

        /// <summary>
        /// Called when the progress changed.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="bitfield">The current bitfield describing the progress.</param>
        void OnProgressChanged(FileHash hash, BitfieldInfo bitfield);

        void OnPieceReady(FileHash hash, int piece);

        void OnPieceCompleted(FileHash hash, int piece);

        void OnBlockReserved(FileHash hash, OmnibusReservationEvent @event);

        void OnBlockExpired(FileHash hash, PeerHash peer, OmnibusBlock block);
    }
}