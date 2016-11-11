using Leak.Core.Client.Events;
using Leak.Core.Common;
using Leak.Core.Scheduler;

namespace Leak.Core.Client.Callbacks
{
    public class PeerClientToScheduler : SchedulerCallbackBase
    {
        private readonly PeerClientContext context;

        public PeerClientToScheduler(PeerClientContext context)
        {
            this.context = context;
        }

        public override void OnMetadataCompleted(FileHash hash)
        {
            context.Bus.Publish("file-discovered", new FileDiscovered
            {
                Hash = hash
            });
        }

        public override void OnResourceInitialized(FileHash hash, Bitfield bitfield)
        {
            context.Bus.Publish("file-initialized", new FileInitialized
            {
                Hash = hash,
                Total = bitfield.Length,
                Completed = bitfield.Completed,
            });
        }

        public override void OnPieceVerified(FileHash hash, PieceInfo piece)
        {
            context.Callback.OnPieceVerified(hash, new PieceVerifiedEvent(hash, piece));
        }

        public override void OnPieceRejected(FileHash hash, PieceInfo piece)
        {
            context.Callback.OnPieceRejected(hash, new PieceRejectedEvent(hash, piece));
        }

        public override void OnDownloadStarted(FileHash hash)
        {
            context.Bus.Publish("file-started", new FileStarted
            {
                Hash = hash
            });
        }

        public override void OnDownloadChanged(FileHash hash, BitfieldInfo bitfield)
        {
            context.Bus.Publish("file-changed", new FileChanged
            {
                Hash = hash,
                Total = bitfield.Total,
                Completed = bitfield.Completed,
            });
        }

        public override void OnDownloadCompleted(FileHash hash)
        {
            context.Bus.Publish("file-completed", new FileCompleted
            {
                Hash = hash
            });
        }
    }
}