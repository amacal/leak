namespace Leak.Core.Repository
{
    public interface RepositoryTask
    {
        void Accept(RepositoryTaskVisitor visitor);

        void Execute(RepositoryContext context);
    }
}