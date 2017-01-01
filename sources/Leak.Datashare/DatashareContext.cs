namespace Leak.Datashare
{
    public class DatashareContext
    {
        private readonly DatashareParameters parameters;
        private readonly DatashareDependencies dependencies;
        private readonly DatashareConfiguration configuration;
        private readonly DatashareHooks hooks;

        public DatashareContext(DatashareParameters parameters, DatashareDependencies dependencies, DatashareConfiguration configuration, DatashareHooks hooks)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.configuration = configuration;
            this.hooks = hooks;
        }

        public DatashareHooks Hooks
        {
            get { return hooks; }
        }

        public DatashareParameters Parameters
        {
            get { return parameters; }
        }

        public DatashareDependencies Dependencies
        {
            get { return dependencies; }
        }

        public DatashareConfiguration Configuration
        {
            get { return configuration; }
        }
    }
}
