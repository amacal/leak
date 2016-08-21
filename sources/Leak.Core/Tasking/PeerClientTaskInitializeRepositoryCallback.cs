using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Repository;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskInitializeRepositoryCallback : RepositoryCallbackBase
    {
        private readonly PeerClientTaskInitializeContext context;

        public PeerClientTaskInitializeRepositoryCallback(PeerClientTaskInitializeContext context)
        {
            this.context = context;
        }

        public override void OnVerified(FileHash hash, Bitfield bitfield)
        {
            PeerClientTaskDownload download = new PeerClientTaskDownload(with =>
            {
                with.Bitfield = bitfield;
                with.Metainfo = context.Metainfo;
                with.Destination = context.Destination;
            });

            context.Queue.Complete(context.Task);
            context.Queue.Register(download);
        }
    }
}