using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Glue.Extensions.Metadata;
using System;

namespace Leak.Core.Metaget
{
    public class MetagetService
    {
        private readonly MetagetContext context;

        public MetagetService(FileHash hash, string destination, MetagetHooks hooks, MetagetConfiguration configuration)
        {
            context = new MetagetContext(hash, destination, hooks, configuration);
        }

        public void Start(LeakPipeline pipeline)
        {
            pipeline.Register(context.Queue);
            pipeline.Register(TimeSpan.FromSeconds(1), OnTick);

            context.Queue.Add(new MetagetTaskVerify());
        }

        public void Stop(LeakPipeline pipeline)
        {
            pipeline.Remove(OnTick);
        }

        public void Dispose()
        {
            context.Queue.Clear();
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
    }
}