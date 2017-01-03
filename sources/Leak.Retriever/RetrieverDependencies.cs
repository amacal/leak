using Leak.Glue;
using Leak.Omnibus;
using Leak.Tasks;

namespace Leak.Retriever
{
    public class RetrieverDependencies
    {
        public LeakPipeline Pipeline;

        public GlueService Glue;

        public OmnibusService Omnibus;

        public RetrieverRepository Repository;
    }
}
