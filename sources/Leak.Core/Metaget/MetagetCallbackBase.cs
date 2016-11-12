using Leak.Core.Common;
using Leak.Core.Metadata;

namespace Leak.Core.Metaget
{
    public abstract class MetagetCallbackBase : MetagetCallback
    {
        public virtual void OnMetadataRequested(PeerHash peer, int piece)
        {
        }

        public virtual void OnMetadataReceived(PeerHash peer, int piece)
        {
        }

        public virtual void OnMetadataCompleted(FileHash hash, Metainfo metainfo)
        {
        }
    }
}