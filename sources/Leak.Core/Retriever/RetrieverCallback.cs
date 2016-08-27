using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public interface RetrieverCallback
    {
        /// <summary>
        /// Called when the retriever was started.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnStarted(FileHash hash);

        /// <summary>
        /// Called when the resource was completed.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnCompleted(FileHash hash);

        /// <summary>
        /// Called when the piece was successfully verified.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="piece">The successfully verified piece.</param>
        void OnPieceVerified(FileHash hash, RetrieverPiece piece);
    }
}