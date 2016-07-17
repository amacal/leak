using Leak.Core.Common;

namespace Leak.Core.Negotiator
{
    public class Handshake
    {
        private readonly PeerHash local;
        private readonly PeerHash remote;
        private readonly FileHash hash;

        public Handshake(PeerHash local, PeerHash remote, FileHash hash)
        {
            this.local = local;
            this.remote = remote;
            this.hash = hash;
        }

        public PeerHash Local
        {
            get { return local; }
        }

        public PeerHash Remote
        {
            get { return remote; }
        }

        public FileHash Hash
        {
            get { return hash; }
        }
    }
}