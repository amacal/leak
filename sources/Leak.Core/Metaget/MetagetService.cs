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
        }

        public void Start()
        {
            context.Timer.Start(OnTick);
            context.Queue.Add(new MetagetTaskVerify());
        }

        public void Stop()
        {
            context.Timer.Stop();
        }

        public void Dispose()
        {
            context.Timer.Dispose();
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
            context.Queue.Process(context);
        }
    }
}