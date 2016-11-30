using Leak.Core.Events;
using System;

namespace Leak.Core.Retriever
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