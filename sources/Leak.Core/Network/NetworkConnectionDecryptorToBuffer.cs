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

        public override void Decrypt(byte[] data, int index, int count)
        {
            inner.Decrypt(data, index, count);
        }
    }
}