using System;

namespace Leak.Core.Network
{
    /// <summary>
    /// Describes the way how the incoming messages should be handled.
    /// </summary>
    public interface NetworkIncomingMessageHandler
    {
        /// <summary>
        /// Called when there is a payload with at least one byte.
        /// </summary>
        /// <param name="message">The incoming message containing the payload.</param>
        void OnMessage(NetworkIncomingMessage message);

        /// <summary>
        /// Called when any exception happened and the channel is not able to
        /// send or receive more messages. The callers should not use this channel
        /// any more.
        /// </summary>
        /// <param name="ex">The exception caused the channel stopped working.</param>
        void OnException(Exception ex);

        /// <summary>
        /// Called when the channel was disconnected and is not able to send or
        /// receive more messages. The callers should not use this channel any more.
        /// </summary>
        void OnDisconnected();
    }
}