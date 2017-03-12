using System;
using Leak.Common;
using Leak.Extensions.Metadata;
using Leak.Tasks;

namespace Leak.Meta.Get
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

        public MetagetHooks Hooks
        {
            get { return context.Hooks; }
        }

        public MetagetParameters Parameters
        {
            get { return context.Parameters; }
        }

        public MetagetDependencies Dependencies
        {
            get { return context.Dependencies; }
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
            context.Queue.Add(() =>
            {
                context.Dependencies.Pipeline.Remove(OnTick);
            });
        }

        public void Handle(MetadataMeasured data)
        {
            context.Queue.Add(new MetagetTaskSize(data.Peer, data.Size));
        }

        public void Handle(MetadataReceived data)
        {
            context.Queue.Add(new MetagetTaskData(data.Peer, data.Piece, data.Data));
        }

        private void OnTick()
        {
            context.Queue.Add(new MetagetTaskNext());
        }

        public void Dispose()
        {
            Stop();
        }
    }
}