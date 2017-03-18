using System.Collections.Concurrent;
using Leak.Common;

namespace Leak.Memory
{
    public class MemoryContext
    {
        private readonly MemoryParameters parameters;
        private readonly MemoryDependencies dependencies;
        private readonly MemoryConfiguration configuration;
        private readonly MemoryHooks hooks;

        private readonly ConcurrentQueue<byte[]> buffer;

        private int count;
        private Size allocation;

        public MemoryContext(MemoryParameters parameters, MemoryDependencies dependencies, MemoryConfiguration configuration, MemoryHooks hooks)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.configuration = configuration;
            this.hooks = hooks;

            count = 0;
            allocation = new Size(0);
            buffer = new ConcurrentQueue<byte[]>();
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

        public ConcurrentQueue<byte[]> Buffer
        {
            get { return buffer; }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public Size Allocation
        {
            get { return allocation; }
            set { allocation = value; }
        }
    }
}