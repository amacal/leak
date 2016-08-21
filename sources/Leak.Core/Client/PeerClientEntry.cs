using Leak.Core.Common;

namespace Leak.Core.Client
{
    public class PeerClientEntry
    {
        private readonly FileHash hash;

        public PeerClientEntry(FileHash hash)
        {
            this.hash = hash;
        }

        public FileHash Hash
        {
            get { return hash; }
        }
    }
}