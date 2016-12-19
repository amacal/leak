using System;
using Leak.Events;

namespace Leak.Retriever
{
    public class RetrieverHooks
    {
        /// <summary>
        /// Called when some data is requested from remote peer.
        /// </summary>
        public Action<BlockRequested> OnBlockRequested;

        /// <summary>
        /// Called when some data is received from remote peer
        /// and successfully handled by the component.
        /// </summary>
        public Action<BlockHandled> OnBlockHandled;

        /// <summary>
        /// Called when some piece was successfully verified against
        /// the hash and the piece is considered to be completed.
        /// </summary>
        public Action<PieceAccepted> OnPieceAccepted;

        /// <summary>
        /// Called when some piece was unsuccessfully verified against
        /// the hash and the piece is considered to be incompleted.
        /// </summary>
        public Action<PieceRejected> OnPieceRejected;

        /// <summary>
        /// Called when some piece was successfully verified and therefore
        /// the retrieving process changed.
        /// </summary>
        public Action<DataChanged> OnDataChanged;

        /// <summary>
        /// Called when the all pieces were successfully retrieved
        /// and validated against file hash. It means there is no
        /// more pieces to download and no more progress can be made.
        /// </summary>
        public Action<DataCompleted> OnDataCompleted;
    }
}