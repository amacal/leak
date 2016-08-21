namespace Leak.Core.Repository
{
    public interface RepositoryTask
    {
        void Execute(RepositoryContext context);
    }
}