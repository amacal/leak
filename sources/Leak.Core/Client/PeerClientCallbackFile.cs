using Leak.Core.Client.Events;
using Leak.Core.Common;

namespace Leak.Core.Client
{
    /// <summary>
    /// Defines handlers related to the FileHash.
    /// </summary>
    public interface PeerClientCallbackFile
    {
        /// <summary>
        /// Called when the resource is scheduled.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnFileScheduled(FileHash hash);

        /// <summary>
        /// Called when the resource is discovered.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnFileDiscovered(FileHash hash);

        /// <summary>
        /// Called when the resource is initialized.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="event">Describes the affected resource.</param>
        void OnFileInitialized(FileHash hash, FileInitializedEvent @event);

        /// <summary>
        /// Called when the downloading started.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnFileStarted(FileHash hash);

        /// <summary>
        /// Called when the downloading progress changed.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="bitfield">The current bitfield describing the progress.</param>
        void OnFileChanged(FileHash hash, BitfieldInfo bitfield);

        /// <summary>
        /// Called when the downloading completed.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        void OnFileCompleted(FileHash hash);
    }
}