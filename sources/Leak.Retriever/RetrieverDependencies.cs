using Leak.Glue;
using Leak.Omnibus;
using Leak.Repository;
using Leak.Tasks;

namespace Leak.Retriever
{
    public class RetrieverDependencies
    {
        public LeakPipeline Pipeline;

        public GlueService Glue;

        public OmnibusService Omnibus;

        public RepositoryService Repository;
    }
}
