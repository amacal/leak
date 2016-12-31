using System;
using Leak.Common;
using Leak.Extensions.Metadata;

namespace Leak.Metaget
{
    public class MetagetService : IDisposable
    {
        private readonly MetagetContext context;

        public MetagetService(MetagetParameters parameters, MetagetDependencies dependencies, MetagetHooks hooks, MetagetConfiguration configuration)
        {
            context = new MetagetContext(parameters, dependencies, hooks, configuration);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public MetagetParameters Parameters
        {
            get { return context.Parameters; }
        }

        public MetagetHooks Hooks
        {
            get { return context.Hooks; }
        }

        public MetagetConfiguration Configuration
        {
            get { return context.Configuration; }
        }

        public void Start()
        {
            context.Dependencies.Pipeline.Register(context.Queue);
            context.Dependencies.Pipeline.Register(TimeSpan.FromSeconds(1), OnTick);

            context.Queue.Add(new MetagetTaskVerify());
        }

        public void Stop()
        {
            context.Dependencies.Pipeline.Remove(OnTick);
        }

        public void HandleMetadataMeasured(MetadataMeasured data)
        {
            context.Queue.Add(new MetagetTaskSize(data.Peer, data.Size));
        }

        public void HandleMetadataReceived(MetadataReceived data)
        {
            context.Queue.Add(new MetagetTaskData(data.Peer, data.Piece, data.Data));
        }

        private void OnTick()
        {
            context.Queue.Add(new MetagetTaskNext());
        }

        public void Dispose()
        {
        }
    }
}