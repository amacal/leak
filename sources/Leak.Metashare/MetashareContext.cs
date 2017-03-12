using Leak.Tasks;

namespace Leak.Meta.Share
{
    public class MetashareContext
    {
        private readonly MetashareParameters parameters;
        private readonly MetashareDependencies dependencies;
        private readonly MetashareConfiguration configuration;
        private readonly MetashareHooks hooks;

        private readonly LeakQueue<MetashareContext> queue;
        private readonly MetashareCollection collection;

        public MetashareContext(MetashareParameters parameters, MetashareDependencies dependencies, MetashareConfiguration configuration, MetashareHooks hooks)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.configuration = configuration;
            this.hooks = hooks;

            collection = new MetashareCollection();
            queue = new LeakQueue<MetashareContext>(this);
        }

        public MetashareParameters Parameters
        {
            get { return parameters; }
        }

        public MetashareDependencies Dependencies
        {
            get { return dependencies; }
        }

        public MetashareConfiguration Configuration
        {
            get { return configuration; }
        }

        public MetashareHooks Hooks
        {
            get { return hooks; }
        }

        public LeakQueue<MetashareContext> Queue
        {
            get { return queue; }
        }

        public MetashareCollection Collection
        {
            get { return collection; }
        }
    }
}