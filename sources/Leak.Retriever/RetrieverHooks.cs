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
    }
}