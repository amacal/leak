using System;
using Leak.Events;

namespace Leak.Data.Map
{
    public class OmnibusHooks
    {
        /// <summary>
        /// Called when some piece was successfully verified and completed
        /// and therefore the retrieving process changed.
        /// </summary>
        public Action<DataChanged> OnDataChanged;

        /// <summary>
        /// Called when the all pieces were successfully retrieved
        /// and validated against file hash. It means there is no
        /// more pieces to download and no more progress can be made.
        /// </summary>
        public Action<DataCompleted> OnDataCompleted;

        /// <summary>
        /// Called when all blocks in the piece are marked as ready
        /// but the piece was not verified against the hash.
        /// </summary>
        public Action<PieceReady> OnPieceReady;

        /// <summary>
        /// Called when all blocks in the piece are marked as ready
        /// and the piece was just verified against the hash.
        /// </summary>
        public Action<PieceCompleted> OnPieceCompleted;

        /// <summary>
        /// Called when the given block was reserved for being downloaded
        /// by some peer.
        /// </summary>
        public Action<BlockReserved> OnBlockReserved;

        /// <summary>
        /// Called when the given block was reserved for being downloaded
        /// by some peer and the peer did not complete it in required time.
        /// </summary>
        public Action<BlockExpired> OnBlockExpired;

        public Action<ThresholdReached> OnThresholdReached;
    }
}