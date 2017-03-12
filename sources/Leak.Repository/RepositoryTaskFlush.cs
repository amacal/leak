namespace Leak.Data.Store
{
    public class RepositoryTaskFlush : RepositoryTask
    {
        public void Execute(RepositoryContext context, RepositoryTaskCallback onCompleted)
        {
            context.View.Flush();
        }

        public bool CanExecute(RepositoryTaskQueue queue)
        {
            return true;
        }

        public void Block(RepositoryTaskQueue queue)
        {
        }

        public void Release(RepositoryTaskQueue queue)
        {
        }
    }
}