namespace Leak.Data.Store
{
    public class RepositoryAllocationRange
    {
        private readonly int from;
        private readonly int to;

        public RepositoryAllocationRange(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        public int From
        {
            get { return from; }
        }

        public int To
        {
            get { return to; }
        }
    }
}