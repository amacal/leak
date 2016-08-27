using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Retriever;

namespace Leak.Core.Scheduler
{
    public interface SchedulerCallback
    {
        /// <summary>
        /// Called when the metadata task completed.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnMetadataCompleted(FileHash hash);

        /// <summary>
        /// Called when the resource was successfully initialized.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="bitfield">The bitfield indicating the progress.</param>
        void OnResourceInitialized(FileHash hash, Bitfield bitfield);

        /// <summary>
        /// Called when a single piece was successfully verified.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="piece">Describes the verified piece.</param>
        void OnPieceVerified(FileHash hash, RetrieverPiece piece);

        /// <summary>
        /// Called when the downloading task started.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnDownloadStarted(FileHash hash);

        /// <summary>
        /// Called when the downloading task completed and
        /// all pieces are downloaded and passed verification.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnDownloadCompleted(FileHash hash);
    }
}