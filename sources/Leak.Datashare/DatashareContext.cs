using Leak.Tasks;

namespace Leak.Data.Share
{
    public class DataShareContext
    {
        private readonly DataShareParameters parameters;
        private readonly DataShareDependencies dependencies;
        private readonly DataShareConfiguration configuration;
        private readonly DataShareHooks hooks;

        private readonly DataShareCollection collection;
        private readonly LeakQueue<DataShareContext> queue;

        private bool verified;

        public DataShareContext(DataShareParameters parameters, DataShareDependencies dependencies, DataShareConfiguration configuration, DataShareHooks hooks)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.configuration = configuration;
            this.hooks = hooks;

            collection = new DataShareCollection();
            queue = new LeakQueue<DataShareContext>(this);
        }

        public DataShareHooks Hooks
        {
            get { return hooks; }
        }

        public DataShareParameters Parameters
        {
            get { return parameters; }
        }

        public DataShareDependencies Dependencies
        {
            get { return dependencies; }
        }

        public DataShareConfiguration Configuration
        {
            get { return configuration; }
        }

        public DataShareCollection Collection
        {
            get { return collection; }
        }

        public LeakQueue<DataShareContext> Queue
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