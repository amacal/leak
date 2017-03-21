using Leak.Common;

namespace Leak.Data.Store
{
    public class RepositoryContext
    {
        private readonly RepositoryParameters parameters;
        private readonly RepositoryDependencies dependencies;
        private readonly RepositoryHooks hooks;
        private readonly RepositoryConfiguration configuration;
        private readonly BitfileService bitfile;
        private readonly RepositoryTaskQueue queue;

        private RepositoryView view;
        private Metainfo metainfo;

        public RepositoryContext(RepositoryParameters parameters, RepositoryDependencies dependencies, RepositoryHooks hooks, RepositoryConfiguration configuration)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.hooks = hooks;
            this.configuration = configuration;

            bitfile = new BitfileService(parameters.Hash, parameters.Destination + ".bitfield");
            queue = new RepositoryTaskQueue(this);
        }

        public RepositoryParameters Parameters
        {
            get { return parameters; }
        }

        public RepositoryDependencies Dependencies
        {
            get { return dependencies; }
        }

        public RepositoryConfiguration Configuration
        {
            get { return configuration; }
        }

        public Metainfo Metainfo
        {
            get { return metainfo; }
            set { metainfo = value; }
        }

        public RepositoryTaskQueue Queue
        {
            get { return queue; }
        }

        public BitfileService Bitfile
        {
            get { return bitfile; }
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