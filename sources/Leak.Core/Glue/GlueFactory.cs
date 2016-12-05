using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Glue
{
    public class GlueFactory
    {
        private readonly DataBlockFactory factory;

        public GlueFactory(DataBlockFactory factory)
        {
            this.factory = factory;
        }

        public GlueService Create(FileHash hash, GlueHooks hooks, GlueConfiguration configuration)
        {
            return new GlueImplementation(hash, factory, hooks, configuration);
        }
    }
}