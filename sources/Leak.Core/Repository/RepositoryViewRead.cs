using Leak.Files;

namespace Leak.Core.Repository
{
    public class RepositoryViewRead
    {
        public int Piece { get; set; }

        public FileBuffer Buffer { get; set; }

        public int Count { get; set; }
    }
}