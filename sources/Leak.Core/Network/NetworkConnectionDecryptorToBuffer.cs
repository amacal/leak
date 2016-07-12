namespace Leak.Core.Network
{
    /// <summary>
    /// Decorates the network connection decryptor with network buffer decryptor
    /// interface to be used by network buffer.
    /// </summary>
    internal class NetworkConnectionDecryptorToBuffer : NetworkBufferDecryptor
    {
        private readonly NetworkConnectionDecryptor inner;

        internal NetworkConnectionDecryptorToBuffer(NetworkConnectionDecryptor inner)
        {
            this.inner = inner;
        }

        public override byte[] Decrypt(byte[] data)
        {
            return inner.Decrypt(data);
        }
    }
}