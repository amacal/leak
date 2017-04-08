using System;
using Leak.Networking.Events;

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
        /// Called when the connection successfully transmitted some bytes
        /// to the remote endpoint.
        /// </summary>
        public Action<ConnectionSent> OnConnectionSent;

        /// <summary>
        /// Called when the connection successfully received some bytes
        /// from the remote endpoint.
        /// </summary>
        public Action<ConnectionReceived> OnConnectionReceived;

        /// <summary>
        /// Called when an existing connection was terminated either
        /// by local or by remote host.
        /// </summary>
        public Action<ConnectionTerminated> OnConnectionTerminated;
    }
}