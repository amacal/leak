namespace Leak.Memory
{
    public class MemoryContext
    {
        private readonly MemoryParameters parameters;
        private readonly MemoryDependencies dependencies;
        private readonly MemoryConfiguration configuration;
        private readonly MemoryHooks hooks;

        private readonly MemoryCollection collection;

        public MemoryContext(MemoryParameters parameters, MemoryDependencies dependencies, MemoryConfiguration configuration, MemoryHooks hooks)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.configuration = configuration;
            this.hooks = hooks;

            collection = new MemoryCollection(this);
        }

        public MemoryParameters Parameters
        {
            get { return parameters; }
        }

        public MemoryDependencies Dependencies
        {
            get { return dependencies; }
        }

        public MemoryConfiguration Configuration
        {
            get { return configuration; }
        }

        public MemoryHooks Hooks
        {
            get { return hooks; }
        }

        public MemoryCollection Collection
        {
            get { return collection; }
        }
    }
}