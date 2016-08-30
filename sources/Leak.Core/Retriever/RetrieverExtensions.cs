using Leak.Core.Repository;

namespace Leak.Core.Retriever
{
    public static class RetrieverExtensions
    {
        public static void Write(this RepositoryService repository, int piece, int offset, byte[] bytes)
        {
            repository.Write(new RepositoryBlockData(piece, offset, bytes));
        }
    }
}