using Leak.Common;
using Leak.Files;

namespace Leak.Repository
{
    public class RepositoryContext
    {
        private readonly Metainfo metainfo;
        private readonly string destination;
        private readonly FileFactory files;
        private readonly RepositoryHooks hooks;
        private readonly RepositoryConfiguration configuration;
        private readonly BitfileService bitfile;
        private readonly RepositoryTaskQueue queue;

        private readonly byte[] buffer;
        private RepositoryView view;

        public RepositoryContext(Metainfo metainfo, string destination, FileFactory files, RepositoryHooks hooks, RepositoryConfiguration configuration)
        {
            this.metainfo = metainfo;
            this.destination = destination;
            this.files = files;
            this.hooks = hooks;
            this.configuration = configuration;

            bitfile = new BitfileService(metainfo.Hash, destination + ".bitfield");

            queue = new RepositoryTaskQueue();
            buffer = new byte[16384];
        }

        public RepositoryConfiguration Configuration
        {
            get { return configuration; }
        }

        public FileFactory Files
        {
            get { return files; }
        }

        public Metainfo Metainfo
        {
            get { return metainfo; }
        }

        public string Destination
        {
            get { return destination; }
        }

        public RepositoryTaskQueue Queue
        {
            get { return queue; }
        }

        public BitfileService Bitfile
        {
            get { return bitfile; }
        }

        public byte[] Buffer
        {
            get { return buffer; }
        }

        public RepositoryView View
        {
            get { return view; }
            set { view = value; }
        }

        public RepositoryHooks Hooks
        {
            get { return hooks; }
        }
    }
}