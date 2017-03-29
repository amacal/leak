using Leak.Tasks;

namespace Leak.Data.Share
{
    public class DatashareContext
    {
        private readonly DatashareParameters parameters;
        private readonly DatashareDependencies dependencies;
        private readonly DatashareConfiguration configuration;
        private readonly DatashareHooks hooks;

        private readonly DatashareCollection collection;
        private readonly LeakQueue<DatashareContext> queue;

        private bool verified;

        public DatashareContext(DatashareParameters parameters, DatashareDependencies dependencies, DatashareConfiguration configuration, DatashareHooks hooks)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.configuration = configuration;
            this.hooks = hooks;

            collection = new DatashareCollection();
            queue = new LeakQueue<DatashareContext>(this);
        }

        public DatashareHooks Hooks
        {
            get { return hooks; }
        }

        public DatashareParameters Parameters
        {
            get { return parameters; }
        }

        public DatashareDependencies Dependencies
        {
            get { return dependencies; }
        }

        public DatashareConfiguration Configuration
        {
            get { return configuration; }
        }

        public DatashareCollection Collection
        {
            get { return collection; }
        }

        public LeakQueue<DatashareContext> Queue
        {
            get { return queue; }
        }

        public bool Verified
        {
            get { return verified; }
            set { verified = value; }
        }
    }
}