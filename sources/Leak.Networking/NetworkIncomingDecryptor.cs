namespace Leak.Networking
{
    /// <summary>
    /// Described the way how the bytes should be decrypted after
    /// receiving them from the remote endpoint.
    /// </summary>
    public interface NetworkIncomingDecryptor
    {
        /// <summary>
        /// Decrypts the given bytes array in place.
        /// </summary>
        /// <param name="data">The array with bytes to decrypt.</param>
        /// <param name="offset">The index to start from.</param>
        /// <param name="count">The number of bytes to decrypt.</param>
        /// <returns>A decrypted array of bytes.</returns>
        void Decrypt(byte[] data, int offset, int count);
    }
}