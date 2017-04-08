namespace Leak.Networking.Core
{
    public interface NetworkIncomingMessage
    {
        /// <summary>
        /// Gets the total number of bytes in the message.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the byte in the message at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index.</param>
        /// <returns>A requested byte value.</returns>
        byte this[int index] { get; }

        /// <summary>
        /// Gets all bytes from the message.
        /// </summary>
        /// <returns>A requested array of bytes.</returns>
        byte[] ToBytes();

        /// <summary>
        /// Gets all the bytes from the message after the given offset.
        /// </summary>
        /// <param name="offset">The number of bytes to skip.</param>
        /// <returns>A requested array of bytes.</returns>
        byte[] ToBytes(int offset);

        /// <summary>
        /// Gets a certain number of bytes from the message after the given offset.
        /// </summary>
        /// <param name="offset">The number of bytes to skip.</param>
        /// <param name="count">The number of bytes to take.</param>
        /// <returns>A requested array of bytes.</returns>
        byte[] ToBytes(int offset, int count);

        DataBlock ToBlock(DataBlockFactory factory, int offset, int count);

        /// <summary>
        /// Acknowledges a portion of the message in order to mark it as handled
        /// and preventing from processing in the future. The message will be
        /// acknowledged from the beginning.
        /// </summary>
        /// <param name="bytes">A number of bytes to acknowledge.</param>
        void Acknowledge(int bytes);

        /// <summary>
        /// Continues handling the message by requesting more bytes from
        /// the remote endpoint. The current message will not be marked as handled
        /// and next message will continue all bytes from current message concatenated
        /// with newly received bytes.
        /// </summary>
        /// <param name="handler">The incoming message handler to use.</param>
        void Continue(NetworkIncomingMessageHandler handler);
    }
}