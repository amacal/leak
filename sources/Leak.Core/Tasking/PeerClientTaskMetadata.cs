using Leak.Core.Common;
using Leak.Core.Metaget;
using System;
using System.IO;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskMetadata : PeerClientTask
    {
        private readonly PeerClientTaskMetadataContext inside;

        public PeerClientTaskMetadata(Action<PeerClientTaskMetadataContext> configurer)
        {
            inside = configurer.Configure(with =>
            {
                with.Task = this;
            });
        }

        public FileHash Hash
        {
            get { return inside.Hash; }
        }

        public PeerClientTaskCallback Start(PeerClientTaskContext context)
        {
            inside.Queue = context.Queue;
            inside.Metaget = new MetagetService(with =>
            {
                with.Hash = inside.Hash;
                with.Destination = Path.Combine(inside.Destination, $"{inside.Hash}.metainfo");
                with.Collector = context.Collector.CreateView(inside.Hash);
                with.Callback = new PeerClientTaskMetadataMetagetCallback(inside);
            });

            inside.Metaget.Start();

            return new PeerClientTaskMetadataTaskCallback(inside);
        }
    }
}