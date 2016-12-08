namespace Leak.Networking
{
    /// <summary>
    /// Described the way how the bytes should be decrypted after
    /// receiving them from the remote endpoint.
    /// </summary>
    public abstract class NetworkConnectionDecryptor
    {
        /// <summary>
        /// Defines a default decryptor which passes bytes
        /// directly from the remote endpoint without any decryption.
        /// </summary>
        public static readonly NetworkConnectionDecryptor Nothing = new NetworkConnectionDecryptorToNothing();

        /// <summary>
        /// Decrypts the given bytes array in place.
        /// </summary>
        /// <param name="data">The array with bytes to decrypt.</param>
        /// <param name="index">The index to start from.</param>
        /// <param name="count">The number of bytes to decrypt.</param>
        /// <returns>A decrypted array of bytes.</returns>
        public abstract void Decrypt(byte[] data, int index, int count);
    }
}