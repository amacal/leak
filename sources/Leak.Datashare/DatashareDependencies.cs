using Leak.Tasks;

namespace Leak.Data.Share
{
    public class DataShareDependencies
    {
        public PipelineService Pipeline;

        public DataShareToDataStore DataStore;

        public DataShareToGlue Glue;

        public DataShareToDataMap DataMap;
    }
}