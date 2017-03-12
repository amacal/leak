using Leak.Tasks;

namespace Leak.Data.Get
{
    public class RetrieverDependencies
    {
        public PipelineService Pipeline;

        public RetrieverGlue Glue;

        public RetrieverOmnibus Omnibus;

        public RetrieverRepository Repository;
    }
}