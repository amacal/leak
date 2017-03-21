using Leak.Tasks;

namespace Leak.Data.Get
{
    public class DataGetDependencies
    {
        public PipelineService Pipeline;

        public DataGetToGlue Glue;

        public DataGetToDataMap Omnibus;

        public DataGetToDataStore Repository;
    }
}