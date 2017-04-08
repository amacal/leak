using System;
using Leak.Common;
using Leak.Events;
using Leak.Networking.Core;

namespace Leak.Data.Store
{
    public class RepositoryService : IDisposable
    {
        private readonly RepositoryContext context;

        public RepositoryService(RepositoryParameters parameters, RepositoryDependencies dependencies, RepositoryHooks hooks, RepositoryConfiguration configuration)
        {
            context = new RepositoryContext(parameters, dependencies, hooks, configuration);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public RepositoryHooks Hooks
        {
            get { return context.Hooks; }
        }

        public RepositoryParameters Parameters
        {
            get { return context.Parameters; }
        }

        public RepositoryDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public RepositoryConfiguration Configuration
        {
            get { return context.Configuration; }
        }

        public void Start()
        {
            context.Dependencies.Pipeline.Register(context.Queue);
        }

        public void Handle(MetafileVerified data)
        {
            context.Metainfo = data.Metainfo;
            context.Queue.Add(new RepositoryTaskAllocate());
        }

        public void Verify(Bitfield scope)
        {
            context.Queue.Add(new RepositoryTaskVerifyRange(scope));
        }

        public void Verify(PieceInfo piece)
        {
            context.Queue.Add(new RepositoryTaskVerifyPiece(piece));
        }

        public void Read(BlockIndex index)
        {
            context.Queue.Add(new RepositoryTaskReadBlock(index));
        }

        public void Write(BlockIndex index, DataBlock data)
        {
            context.Queue.Add(new RepositoryTaskWriteBlock(index, data));
        }

        public void Flush()
        {
            context.Queue.Add(new RepositoryTaskFlush());
        }

        public void Dispose()
        {
            context.Queue.Stop();
            context.View?.Dispose();
        }
    }
}