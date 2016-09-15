namespace Leak.Core.Repository
{
    public interface RepositoryTaskVisitor
    {
        void Visit(RepositoryTaskAllocate task);

        void Visit(RepositoryTaskVerifyPiece task);

        void Visit(RepositoryTaskVerifyRange task);

        void Visit(RepositoryTaskWriteBlock task);
    }
}