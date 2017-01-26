using Leak.Files;

namespace Leak.Datastore
{
    public class RepositoryViewWrite
    {
        public int Piece { get; set; }

        public int Block { get; set; }

        public FileBuffer Buffer { get; set; }

        public int Count { get; set; }
    }
}