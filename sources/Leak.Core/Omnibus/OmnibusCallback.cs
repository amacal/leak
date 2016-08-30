using Leak.Core.Common;

namespace Leak.Core.Omnibus
{
    public interface OmnibusCallback
    {
        /// <summary>
        /// Called when the progress changed.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="bitfield">The current bitfield describing the progress.</param>
        void OnChanged(FileHash hash, BitfieldInfo bitfield);
    }
}