using Leak.Core.Common;
using Leak.Core.Repository;
using System;
using System.IO;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskInitialize : PeerClientTask
    {
        private readonly PeerClientTaskInitializeContext inside;

        public PeerClientTaskInitialize(Action<PeerClientTaskInitializeContext> configurer)
        {
            inside = configurer.Configure(with =>
            {
                with.Task = this;
            });
        }

        public FileHash Hash
        {
            get { return inside.Metainfo.Hash; }
        }

        public PeerClientTaskCallback Start(PeerClientTaskContext context)
        {
            inside.Queue = context.Queue;
            inside.Repository = new RepositoryService(with =>
            {
                with.Metainfo = inside.Metainfo;
                with.Destination = Path.Combine(inside.Destination, $"{inside.Metainfo.Hash}");
                with.Callback = new PeerClientTaskInitializeRepositoryCallback(inside);
            });

            inside.Repository.Allocate();
            inside.Repository.Verify();

            return new PeerClientTaskCallbackNothing();
        }
    }
}