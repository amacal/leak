using System;
using Leak.Common;
using Leak.Events;

namespace Leak.Datashare
{
    public class DatashareService : IDisposable
    {
        private readonly DatashareContext context;

        public DatashareService(DatashareParameters parameters, DatashareDependencies dependencies, DatashareConfiguration configuration, DatashareHooks hooks)
        {
            context = new DatashareContext(parameters, dependencies, configuration, hooks);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public DatashareHooks Hooks
        {
            get { return context.Hooks; }
        }

        public DatashareParameters Parameters
        {
            get { return context.Parameters; }
        }

        public DatashareDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public DatashareConfiguration Configuration
        {
            get { return context.Configuration; }
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Handle(BlockRequested data)
        {
        }

        public void Dispose()
        {
        }
    }
}
