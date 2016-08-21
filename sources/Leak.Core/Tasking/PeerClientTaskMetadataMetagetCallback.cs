using Leak.Core.Common;
using Leak.Core.Metadata;
using Leak.Core.Metaget;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskMetadataMetagetCallback : MetagetCallbackBase
    {
        private readonly PeerClientTaskMetadataContext context;

        public PeerClientTaskMetadataMetagetCallback(PeerClientTaskMetadataContext context)
        {
            this.context = context;
        }

        public override void OnMetadataCompleted(FileHash hash, Metainfo metainfo)
        {
            PeerClientTaskInitialize initialize = new PeerClientTaskInitialize(with =>
            {
                with.Metainfo = metainfo;
                with.Destination = context.Destination;
            });

            context.Metaget.Stop();
            context.Metaget.Dispose();

            context.Queue.Complete(context.Task);
            context.Queue.Register(initialize);
        }
    }
}