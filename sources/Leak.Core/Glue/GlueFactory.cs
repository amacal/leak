using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Glue
{
    public class GlueFactory
    {
        private readonly DataBlockFactory factory;
        private readonly GlueConfiguration configuration;

        public GlueFactory(DataBlockFactory factory, GlueConfiguration configuration)
        {
            this.factory = factory;
            this.configuration = configuration;
        }

        public GlueService Create(FileHash hash, GlueHooks hooks)
        {
            return new GlueImplementation(hash, factory, hooks, configuration);
        }
    }
}