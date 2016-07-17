using Leak.Core.Metadata;

namespace Leak.Core.Repository
{
    public class ResourceRepository
    {
        private readonly Metainfo metainfo;
        private readonly string location;

        public ResourceRepository(Metainfo metainfo, string location)
        {
            this.metainfo = metainfo;
            this.location = location;
        }
    }
}