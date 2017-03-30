using Leak.Common;
using Leak.Tasks;

namespace Leak.Data.Share
{
    public class DataShareBuilder
    {
        private readonly DataShareParameters parameters;
        private readonly DataShareDependencies dependencies;
        private readonly DataShareConfiguration configuration;

        public DataShareBuilder()
        {
            parameters = new DataShareParameters();
            dependencies = new DataShareDependencies();
            configuration = new DataShareConfiguration();
        }

        public DataShareBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public DataShareBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public DataShareBuilder WithDataMap(DataShareToDataMap dataMap)
        {
            dependencies.DataMap = dataMap;
            return this;
        }

        public DataShareBuilder WithDataStore(DataShareToDataStore dataStore)
        {
            dependencies.DataStore = dataStore;
            return this;
        }

        public DataShareBuilder WithGlue(DataShareToGlue glue)
        {
            dependencies.Glue = glue;
            return this;
        }

        public DataShareService Build()
        {
            return Build(new DataShareHooks());
        }

        public DataShareService Build(DataShareHooks hooks)
        {
            return new DataShareService(parameters, dependencies, configuration, hooks);
        }
    }
}