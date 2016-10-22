using Leak.Core.Common;
using System;

namespace Leak.Core.Repository
{
    public class RepositoryService
    {
        private readonly RepositoryContext context;

        public RepositoryService(Action<RepositoryConfiguration> configurer)
        {
            context = new RepositoryContext(null, configurer);
        }

        public RepositoryService(RepositoryService service, Action<RepositoryConfiguration> configurer)
        {
            context = new RepositoryContext(service.context, configurer);
        }

        public void Start()
        {
            context.Queue.Start(context);
        }

        public void Allocate()
        {
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
    }
}