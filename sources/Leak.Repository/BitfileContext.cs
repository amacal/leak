using Leak.Common;

namespace Leak.Data.Store
{
    public class BitfileContext
    {
        private readonly FileHash hash;
        private readonly string path;
        private readonly BitfileDestination destination;

        public BitfileContext(FileHash hash, string path)
        {
            this.hash = hash;
            this.path = path;

            this.destination = new BitfileDestination(this);
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public string Path
        {
            get { return path; }
        }

        public BitfileDestination Destination
        {
            get { return destination; }
        }
    }
}