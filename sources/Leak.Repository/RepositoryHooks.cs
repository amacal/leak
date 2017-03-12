using System;
using Leak.Events;

namespace Leak.Data.Store
{
    public class RepositoryHooks
    {
        /// <summary>
        /// Called when repository data was successfully allocated
        /// in the destination directory and there was a change
        /// which affected the file system.
        /// </summary>
        public Action<DataAllocated> OnDataAllocated;

        /// <summary>
        /// Called when repository data was verified against metafile
        /// and the actual bitfield is known.
        /// </summary>
        public Action<DataVerified> OnDataVerified;

        /// <summary>
        /// Called when some block was successfully read from the
        /// persisted form in the file system.
        /// </summary>
        public Action<BlockRead> OnBlockRead;

        /// <summary>
        /// Called when some block of some piece was successfully
        /// persisted in the file system.
        /// </summary>
        public Action<BlockWritten> OnBlockWritten;

        /// <summary>
        /// Called when some piece was successfully verified against
        /// the hash and there is no need for changing the piece.
        /// </summary>
        public Action<PieceAccepted> OnPieceAccepted;

        /// <summary>
        /// Called when some piece was unsuccessfully verified against
        /// the hash and it should be once again written and verified.
        /// </summary>
        public Action<PieceRejected> OnPieceRejected;
    }
}