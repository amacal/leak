namespace Leak.Core.Repository
{
    public class RepositoryPiece
    {
        private readonly int index;

        public RepositoryPiece(int index)
        {
            this.index = index;
        }

        public int Index
        {
            get { return index; }
        }
    }
}