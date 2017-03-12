using Leak.Files;

namespace Leak.Data.Store
{
    public class RepositoryViewEntry
    {
        public File File { get; set; }

        public long Size { get; set; }

        public long Start { get; set; }

        public long End { get; set; }
    }
}