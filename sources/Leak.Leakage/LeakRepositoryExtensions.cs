using Leak.Common;
using Leak.Repository;
using Leak.Retriever;

namespace Leak.Leakage
{
    public static class LeakRepositoryExtensions
    {
        public static RetrieverRepository ToRetriever(this RepositoryService repository)
        {
            return new RepositoryToRetriever(repository);
        }

        private class RepositoryToRetriever : RetrieverRepository
        {
            private readonly RepositoryService repository;

            public RepositoryToRetriever(RepositoryService repository)
            {
                this.repository = repository;
            }

            public void Verify(PieceInfo piece)
            {
                repository.Verify(piece);
            }

            public void Write(BlockIndex block, DataBlock data)
            {
                repository.Write(block, data);
            }
        }
    }
}
