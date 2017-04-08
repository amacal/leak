using Leak.Networking.Core;

namespace Leak.Networking
{
    /// <summary>
    /// Described the way how the bytes should be encrypted before
    /// transmitting them to the remote endpoint.
    /// </summary>
    public interface NetworkOutgoingEncryptor
    {
        /// <summary>
        /// Encrypts the given bytes array.
        /// </summary>
        /// <param name="block">The array with bytes to encrypt.</param>
        void Encrypt(DataBlock block);

        /// <summary>
        /// Encrypts the given bytes array in place.
        /// </summary>
        /// <param name="data">The array with bytes to decrypt.</param>
        /// <param name="offset">The index to start from.</param>
        /// <param name="count">The number of bytes to decrypt.</param>
        /// <returns>A decrypted array of bytes.</returns>
        void Encrypt(byte[] data, int offset, int count);
    }
}