using System;
using Leak.Extensions.Metadata;
using Leak.Glue;
using Leak.Tasks;

namespace Leak.Metaget
{
    public class MetagetService : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly MetagetContext context;

        public MetagetService(LeakPipeline pipeline, GlueService glue, string destination, MetagetHooks hooks, MetagetConfiguration configuration)
        {
            this.pipeline = pipeline;
            context = new MetagetContext(glue, destination, hooks, configuration);
        }

        public void Start()
        {
            pipeline.Register(context.Queue);
            pipeline.Register(TimeSpan.FromSeconds(1), OnTick);

            context.Queue.Add(new MetagetTaskVerify());
        }

        public void Stop()
        {
            pipeline.Remove(OnTick);
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