using System;
using Leak.Events;

namespace Leak.Data.Get
{
    public class RetrieverHooks
    {
        /// <summary>
        /// Called when some block is requested from remote peer.
        /// </summary>
        public Action<BlockRequested> OnBlockRequested;

        /// <summary>
        /// Called when some block is received from remote peer
        /// and successfully handled by the component.
        /// </summary>
        public Action<BlockHandled> OnBlockHandled;
    }
}