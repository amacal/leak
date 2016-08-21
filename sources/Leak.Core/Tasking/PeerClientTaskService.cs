using System;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskService
    {
        private readonly PeerClientTaskContext context;

        public PeerClientTaskService(Action<PeerClientTaskConfiguration> configurer)
        {
            context = new PeerClientTaskContext(configurer);
        }

        public void Handle(Action<PeerClientTaskCallback> callback)
        {
            context.Queue.Notify(callback);
        }

        public void Metadata(Action<PeerClientTaskMetadataContext> configurer)
        {
            context.Queue.Register(new PeerClientTaskMetadata(configurer));
        }

        public void Initialize(Action<PeerClientTaskInitializeContext> configurer)
        {
            context.Queue.Register(new PeerClientTaskInitialize(configurer));
        }
    }
}