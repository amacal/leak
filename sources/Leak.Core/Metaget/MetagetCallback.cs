using Leak.Core.Common;

namespace Leak.Core.Metaget
{
    public interface MetagetCallback
    {
        void OnMetadataMeasured(PeerHash peer, int size);

        void OnMetadataRequested(PeerHash peer, int piece);

        void OnMetadataReceived(PeerHash peer, int piece);

        void OnMetadataCompleted(FileHash hash);
    }
}