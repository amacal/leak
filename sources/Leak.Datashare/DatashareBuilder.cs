using Leak.Common;
using Leak.Data.Store;
using Leak.Glue;
using Leak.Tasks;

namespace Leak.Data.Share
{
    public class DatashareBuilder
    {
        private readonly DatashareParameters parameters;
        private readonly DatashareDependencies dependencies;
        private readonly DatashareConfiguration configuration;

        public DatashareBuilder()
        {
            parameters = new DatashareParameters();
            dependencies = new DatashareDependencies();
            configuration = new DatashareConfiguration();
        }

        public DatashareBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public DatashareBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public DatashareBuilder WithDataMap(DataShareToDataMap dataMap)
        {
            dependencies.DataMap = dataMap;
            return this;
        }

        public DatashareBuilder WithDataStore(RepositoryService dataStore)
        {
            dependencies.DataStore = dataStore;
            return this;
        }

        public DatashareBuilder WithGlue(GlueService glue)
        {
            dependencies.Glue = glue;
            return this;
        }

        public DatashareService Build()
        {
            return Build(new DatashareHooks());
        }

        public DatashareService Build(DatashareHooks hooks)
        {
            return new DatashareService(parameters, dependencies, configuration, hooks);
        }
    }
}