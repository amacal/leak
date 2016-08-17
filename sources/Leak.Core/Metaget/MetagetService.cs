using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using System;

namespace Leak.Core.Metaget
{
    public class MetagetService
    {
        private readonly MetagetContext context;

        public MetagetService(Action<MetagetConfiguration> configurer)
        {
            context = new MetagetContext(configurer);
            context.Timer.Start(OnTick);
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
            context.Queue.Process(context);
        }
    }
}