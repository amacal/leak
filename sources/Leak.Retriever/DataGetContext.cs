using Leak.Tasks;

namespace Leak.Data.Get
{
    public class DataGetContext
    {
        private readonly DataGetParameters parameters;
        private readonly DataGetDependencies dependencies;
        private readonly DataGetConfiguration configuration;
        private readonly DataGetHooks hooks;

        private readonly LeakQueue<DataGetContext> queue;
        private bool verified;

        public DataGetContext(DataGetParameters parameters, DataGetDependencies dependencies, DataGetConfiguration configuration, DataGetHooks hooks)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.configuration = configuration;
            this.hooks = hooks;

            queue = new LeakQueue<DataGetContext>(this);
        }

        public DataGetHooks Hooks
        {
            get { return hooks; }
        }

        public DataGetParameters Parameters
        {
            get { return parameters; }
        }

        public DataGetDependencies Dependencies
        {
            get { return dependencies; }
        }

        public DataGetConfiguration Configuration
        {
            get { return configuration; }
        }

        public LeakQueue<DataGetContext> Queue
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