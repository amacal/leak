using Leak.Core.Events;
using System;

namespace Leak.Core.Retriever
{
    public class RetrieverHooks
    {
        /// <summary>
        /// Called when some data is requested from remote peer.
        /// </summary>
        public Action<DataRequested> OnDataRequested;

        /// <summary>
        /// Called when some data is received from remote peer.
        /// </summary>
        public Action<DataReceived> OnDataReceived;

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