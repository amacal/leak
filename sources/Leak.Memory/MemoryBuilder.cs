namespace Leak.Memory
{
    public class MemoryBuilder
    {
        private readonly MemoryParameters parameters;
        private readonly MemoryDependencies dependencies;
        private readonly MemoryConfiguration configuration;

        public MemoryBuilder()
        {
            parameters = new MemoryParameters();
            dependencies = new MemoryDependencies();
            configuration = new MemoryConfiguration();
        }

        public MemoryBuilder WithMaxRequestSize(int maxRequestSize)
        {
            configuration.MaxBlockSize = maxRequestSize;
            return this;
        }

        public MemoryBuilder WithThresholds(params int[] thresholds)
        {
            configuration.Thresholds = thresholds;
            return this;
        }

        public MemoryService Build()
        {
            return Build(new MemoryHooks());
        }

        public MemoryService Build(MemoryHooks hooks)
        {
            return new MemoryService(parameters, dependencies, configuration, hooks);
        }
    }
}