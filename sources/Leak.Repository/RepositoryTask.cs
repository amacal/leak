namespace Leak.Data.Store
{
    public interface RepositoryTask
    {
        bool CanExecute(RepositoryTaskQueue queue);

        void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted);

        void Block(RepositoryTaskQueue queue);

        void Release(RepositoryTaskQueue queue);
    }
}