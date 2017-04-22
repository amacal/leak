namespace Leak.Peer.Coordinator
{
    public class CoordinatorContext
    {
        private readonly CoordinatorParameters parameters;
        private readonly CoordinatorDependencies dependencies;
        private readonly CoordinatorHooks hooks;
        private readonly CoordinatorConfiguration configuration;

        private readonly CoordinatorFacts facts;
        private readonly CoordinatorCollection collection;

        public CoordinatorContext(CoordinatorParameters parameters, CoordinatorDependencies dependencies, CoordinatorHooks hooks, CoordinatorConfiguration configuration)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.hooks = hooks;
            this.configuration = configuration;

            collection = new CoordinatorCollection();
            facts = new CoordinatorFacts();
        }

        public CoordinatorParameters Parameters
        {
            get { return parameters; }
        }

        public CoordinatorDependencies Dependencies
        {
            get { return dependencies; }
        }

        public CoordinatorHooks Hooks
        {
            get { return hooks; }
        }

        public CoordinatorConfiguration Configuration
        {
            get { return configuration; }
        }

        public CoordinatorFacts Facts
        {
            get { return facts; }
        }

        public CoordinatorCollection Collection
        {
            get { return collection; }
        }
    }
}