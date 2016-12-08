namespace Leak.Networking
{
    /// <summary>
    /// Described the way how the bytes should be encrypted before
    /// transmitting them to the remote endpoint.
    /// </summary>
    public abstract class NetworkConnectionEncryptor
    {
        /// <summary>
        /// Defines a default encryptor which passes bytes
        /// directly to the remote endpoint without any encryption.
        /// </summary>
        public static readonly NetworkConnectionEncryptor Nothing = new NetworkConnectionEncryptorToNothing();

        /// <summary>
        /// Encrypts the given bytes array.
        /// </summary>
        /// <param name="data">The array with bytes to encrypt.</param>
        /// <returns>An encrypted array of bytes.</returns>
        public abstract byte[] Encrypt(byte[] data);
    }
}