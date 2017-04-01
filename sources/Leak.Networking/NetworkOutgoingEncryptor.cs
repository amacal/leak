using Leak.Common;

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
    }
}