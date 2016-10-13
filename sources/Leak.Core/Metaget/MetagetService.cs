using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Core;
using System;

namespace Leak.Core.Metaget
{
    public class MetagetService
    {
        private readonly MetagetContext context;

        public MetagetService(Action<MetagetConfiguration> configurer)
        {
            context = new MetagetContext(configurer);
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

        public void OnSize(PeerHash peer, MetadataSize size)
        {
            context.Queue.Add(new MetagetTaskSize(peer, size));
        }

        public void OnData(PeerHash peer, MetadataData data)
        {
            context.Queue.Add(new MetagetTaskData(peer, data));
        }

        private void OnTick()
        {
            context.Queue.Add(new MetagetTaskNext());
        }
    }
}