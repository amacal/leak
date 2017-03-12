using Leak.Tasks;

namespace Leak.Meta.Store
{
    public class MetafileContext
    {
        private readonly MetafileParameters parameters;
        private readonly MetafileDependencies dependencies;
        private readonly MetafileHooks hooks;
        private readonly MetafileDestination destination;
        private readonly LeakQueue<MetafileContext> queue;

        private bool isCompleted;
        private int totalSize;

        public MetafileContext(MetafileParameters parameters, MetafileDependencies dependencies, MetafileHooks hooks)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.hooks = hooks;

            queue = new LeakQueue<MetafileContext>(this);
            destination = new MetafileDestination(this);
        }

        public bool IsCompleted
        {
            get { return isCompleted; }
            set { isCompleted = value; }
        }

        public int TotalSize
        {
            get { return totalSize; }
            set { totalSize = value; }
        }

        public MetafileDestination Destination
        {
            get { return destination; }
        }

        public MetafileParameters Parameters
        {
            get { return parameters; }
        }

        public MetafileHooks Hooks
        {
            get { return hooks; }
        }

        public MetafileDependencies Dependencies
        {
            get { return dependencies; }
        }

        public LeakQueue<MetafileContext> Queue
        {
            get { return queue; }
        }
    }
}