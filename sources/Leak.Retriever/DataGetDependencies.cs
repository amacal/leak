using Leak.Tasks;

namespace Leak.Data.Get
{
    public class DataGetDependencies
    {
        public PipelineService Pipeline;

        public DataGetToGlue Glue;

        public DataGetToDataMap DataMap;

        public DataGetToDataStore DataStore;
    }
}