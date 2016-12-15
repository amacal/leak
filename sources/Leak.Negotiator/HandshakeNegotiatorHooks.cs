using Leak.Events;
using System;

namespace Leak.Negotiator
{
    public class HandshakeNegotiatorHooks
    {
        /// <summary>
        /// Called when the handshake was successfully completed.
        /// </summary>
        public Action<HandshakeCompleted> OnHandshakeCompleted;

        /// <summary>
        /// Called when the handshake failed and the remote peer was rejected.
        /// </summary>
        public Action<HandshakeRejected> OnHandshakeRejected;
    }
}