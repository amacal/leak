namespace Leak.Core.Repository
{
    public class RepositoryTaskBuffer
    {
        public bool CanExecute(RepositoryTaskQueue queue)
        {
            return queue.IsBlocked("all") == false;
        }

        public void Block(RepositoryTaskQueue queue)
        {
            queue.Block("all");
        }

        public void Release(RepositoryTaskQueue queue)
        {
            queue.Release("all");
        }
    }
}