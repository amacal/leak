using System;

namespace Leak.Core.Net
{
    public class PeerBufferConfiguration
    {
        public int Size { get; set; }

        public Func<byte[], byte[]> Decrypt { get; set; }
    }
}