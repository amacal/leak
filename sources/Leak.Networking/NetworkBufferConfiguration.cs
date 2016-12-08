namespace Leak.Networking
{
    /// <summary>
    /// Describes the configuration of the network buffer.
    /// </summary>
    public class NetworkBufferConfiguration
    {
        /// <summary>
        /// The requested size.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The way how the incoming messages should be decrypted.
        /// </summary>
        public NetworkBufferDecryptor Decryptor { get; set; }

        /// <summary>
        /// The object to used for multi-thread synchronization.
        /// </summary>
        public object Synchronized { get; set; }
    }
}