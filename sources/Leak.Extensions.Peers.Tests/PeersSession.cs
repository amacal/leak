using System;
using Leak.Common;

namespace Leak.Extensions.Peers.Tests
{
    public class PeersSession : IDisposable
    {
        private readonly FileHash hash;
        private readonly PeersSide left;
        private readonly PeersSide right;

        public PeersSession(FileHash hash, PeersSide left, PeersSide right)
        {
            this.hash = hash;
            this.left = left;
            this.right = right;
        }

        public PeersSide Left
        {
            get { return left; }
        }

        public PeersSide Right
        {
            get { return right; }
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public void Dispose()
        {
            left.Dispose();
            right.Dispose();
        }
    }
}
