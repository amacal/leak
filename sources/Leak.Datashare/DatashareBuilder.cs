using Leak.Common;
using Leak.Glue;
using Leak.Repository;

namespace Leak.Datashare
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

        public DatashareBuilder WithRepository(RepositoryService repository)
        {
            dependencies.Repository = repository;
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
