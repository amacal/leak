using Leak.Common;

namespace Leak.Networking
{
    /// <summary>
    /// Described the way how the bytes should be encrypted before
    /// transmitting them to the remote endpoint.
    /// </summary>
    public abstract class NetworkEncryptor
    {
        /// <summary>
        /// Defines a default encryptor which passes bytes
        /// directly to the remote endpoint without any encryption.
        /// </summary>
        public static readonly NetworkEncryptor Nothing = new NetworkEncryptorToNothing();

        /// <summary>
        /// Encrypts the given bytes array.
        /// </summary>
        /// <param name="block">The array with bytes to encrypt.</param>
        public abstract void Encrypt(DataBlock block);
    }
}