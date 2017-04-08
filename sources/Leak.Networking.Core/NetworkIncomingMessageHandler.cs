namespace Leak.Networking.Core
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
        /// Called when the channel was disconnected and is not able to send or
        /// receive more messages. The callers should not use this channel any more.
        /// </summary>
        void OnDisconnected();
    }
}