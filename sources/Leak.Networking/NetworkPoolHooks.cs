using Leak.Events;
using System;

namespace Leak.Networking
{
    public class NetworkPoolHooks
    {
        /// <summary>
        /// Called when a socket was converted to the connection.
        /// </summary>
        public Action<ConnectionAttached> OnConnectionAttached;

        /// <summary>
        /// Called when an existing connection was switched into
        /// encryption mode.
        /// </summary>
        public Action<ConnectionEncrypted> OnConnectionEncrypted;

        /// <summary>
        /// Called when an existing connection was terminated either
        /// by local or by remote host.
        /// </summary>
        public Action<ConnectionTerminated> OnConnectionTerminated;
    }
}