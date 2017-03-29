using Leak.Data.Store;
using Leak.Glue;
using Leak.Tasks;

namespace Leak.Data.Share
{
    public class DatashareDependencies
    {
        public PipelineService Pipeline;

        public RepositoryService DataStore;

        public GlueService Glue;

        public DataShareToDataMap DataMap;
    }
}