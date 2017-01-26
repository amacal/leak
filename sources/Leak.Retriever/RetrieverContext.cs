using Leak.Tasks;

namespace Leak.Dataget
{
    public class RetrieverContext
    {
        private readonly RetrieverParameters parameters;
        private readonly RetrieverDependencies dependencies;
        private readonly RetrieverConfiguration configuration;
        private readonly RetrieverHooks hooks;

        private readonly LeakQueue<RetrieverContext> queue;

        public RetrieverContext(RetrieverParameters parameters, RetrieverDependencies dependencies, RetrieverConfiguration configuration, RetrieverHooks hooks)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.configuration = configuration;
            this.hooks = hooks;

            queue = new LeakQueue<RetrieverContext>(this);
        }

        public RetrieverHooks Hooks
        {
            get { return hooks; }
        }

        public RetrieverParameters Parameters
        {
            get { return parameters; }
        }

        public RetrieverDependencies Dependencies
        {
            get { return dependencies; }
        }

        public RetrieverConfiguration Configuration
        {
            get { return configuration; }
        }

        public LeakQueue<RetrieverContext> Queue
        {
            get { return queue; }
        }
    }
}