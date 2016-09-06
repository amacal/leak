using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public interface RetrieverCallback
    {
        /// <summary>
        /// Called when the retriever was started.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnFileStarted(FileHash hash);

        /// <summary>
        /// Called when the progress changed.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="bitfield">The current bitfield describing the progress.</param>
        void OnFileChanged(FileHash hash, BitfieldInfo bitfield);

        /// <summary>
        /// Called when the resource was completed.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnFileCompleted(FileHash hash);

        /// <summary>
        /// Called when the piece was successfully verified.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="piece">The successfully verified piece.</param>
        void OnPieceVerified(FileHash hash, PieceInfo piece);
    }
}