using Leak.Core.Metadata;
using Leak.Files;

namespace Leak.Core.Repository
{
    public class RepositoryConfiguration
    {
        public Metainfo Metainfo { get; set; }

        public string Destination { get; set; }

        public FileFactory Files { get; set; }

        public RepositoryCallback Callback { get; set; }
    }
}