using Leak.Common;
using Leak.Dataget;
using Leak.Datastore;

namespace Leak.Leakage
{
    public static class LeakRepositoryExtensions
    {
        public static RetrieverRepository ToRetriever(this RepositoryService service)
        {
            return new RepositoryToRetriever(service);
        }

        private class RepositoryToRetriever : RetrieverRepository
        {
            private readonly RepositoryService service;

            public RepositoryToRetriever(RepositoryService service)
            {
                this.service = service;
            }

            public void Verify(PieceInfo piece)
            {
                service.Verify(piece);
            }

            public void Write(BlockIndex block, DataBlock data)
            {
                service.Write(block, data);
            }
        }
    }
}