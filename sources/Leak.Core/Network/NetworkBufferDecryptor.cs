namespace Leak.Core.Network
{
    /// <summary>
    /// Described the way how the bytes should be decrypted after
    /// receiving them from the remote endpoint.
    /// </summary>
    public abstract class NetworkBufferDecryptor
    {
        /// <summary>
        /// Defines a default decryptor which passes bytes
        /// directly from the remote endpoint without any decryption.
        /// </summary>
        public static readonly NetworkBufferDecryptor Nothing = new NetworkBufferDecryptorToNothing();

        /// <summary>
        /// Decrypts the given bytes array.
        /// </summary>
        /// <param name="data">The array with bytes to decrypt.</param>
        /// <returns>A decrypted array of bytes.</returns>
        public abstract byte[] Decrypt(byte[] data);
    }
}