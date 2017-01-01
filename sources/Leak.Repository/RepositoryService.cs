using System;
using Leak.Common;
using Leak.Events;

namespace Leak.Repository
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
            context.Queue.Start(context);
            context.Queue.Add(new RepositoryTaskAllocate());
        }

        public void Handle(MetadataDiscovered data)
        {
            context.Metainfo = data.Metainfo;
        }

        public void Verify(Bitfield scope)
        {
            context.Queue.Add(new RepositoryTaskVerifyRange(scope));
        }

        public void Verify(PieceInfo piece)
        {
            context.Queue.Add(new RepositoryTaskVerifyPiece(piece));
        }

        public void Write(RepositoryBlockData block)
        {
            context.Queue.Add(new RepositoryTaskWriteBlock(block));
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