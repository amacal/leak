using Leak.Core.Common;
using Leak.Core.Metadata;

namespace Leak.Core.Metaget
{
    public interface MetagetCallback
    {
        void OnMetadataRequested(PeerHash peer, int piece);

        void OnMetadataReceived(PeerHash peer, int piece);

        void OnMetadataCompleted(FileHash hash, Metainfo metainfo);
    }
}