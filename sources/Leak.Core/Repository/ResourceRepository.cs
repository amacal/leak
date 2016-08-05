using Leak.Core.Messages;
using Leak.Core.Metadata;

namespace Leak.Core.Repository
{
    public interface ResourceRepository
    {
        MetainfoProperties Properties { get; }

        Bitfield Initialize();

        ResourceRepository WithMetainfo(out Metainfo metainfo);

        ResourceRepositorySession OpenSession();
    }
}