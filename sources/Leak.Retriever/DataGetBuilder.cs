using Leak.Common;
using Leak.Tasks;

namespace Leak.Data.Get
{
    public class DataGetBuilder
    {
        private readonly DataGetParameters parameters;
        private readonly DataGetDependencies dependencies;
        private readonly DataGetConfiguration configuration;

        public DataGetBuilder()
        {
            parameters = new DataGetParameters();
            dependencies = new DataGetDependencies();
            configuration = new DataGetConfiguration();
        }

        public DataGetBuilder WithHash(FileHash hash)
        {
            parameters.Hash = hash;
            return this;
        }

        public DataGetBuilder WithPipeline(PipelineService pipeline)
        {
            dependencies.Pipeline = pipeline;
            return this;
        }

        public DataGetBuilder WithGlue(DataGetToGlue glue)
        {
            dependencies.Glue = glue;
            return this;
        }

        public DataGetBuilder WithDataMap(DataGetToDataMap dataMap)
        {
            dependencies.DataMap = dataMap;
            return this;
        }

        public DataGetBuilder WithDataStore(DataGetToDataStore dataStore)
        {
            dependencies.DataStore = dataStore;
            return this;
        }

        public DataGetBuilder WithStrategy(string strategy)
        {
            configuration.Strategy = strategy;
            return this;
        }

        public DataGetService Build()
        {
            return Build(new DataGetHooks());
        }

        public DataGetService Build(DataGetHooks hooks)
        {
            return new DataGetService(parameters, dependencies, configuration, hooks);
        }
    }
}