using Leak.Common;
using Leak.Files;
using System;

namespace Leak.Core.Repository
{
    public class RepositoryService : IDisposable
    {
        private readonly RepositoryContext context;

        public RepositoryService(Metainfo metainfo, string destination, FileFactory files, RepositoryHooks hooks, RepositoryConfiguration configuration)
        {
            context = new RepositoryContext(metainfo, destination, files, hooks, configuration);
        }

        public void Start()
        {
            context.Queue.Start(context);
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