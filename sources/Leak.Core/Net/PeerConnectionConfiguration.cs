using System;

namespace Leak.Core.Net
{
    public class PeerConnectionConfiguration
    {
        public Func<byte[], byte[]> Encrypt { get; set; }

        public Func<byte[], byte[]> Decrypt { get; set; }
    }
}