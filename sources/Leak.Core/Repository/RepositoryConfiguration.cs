using Leak.Core.Metadata;

namespace Leak.Core.Repository
{
    public class RepositoryConfiguration
    {
        public Metainfo Metainfo { get; set; }

        public string Destination { get; set; }

        public RepositoryCallback Callback { get; set; }
    }
}