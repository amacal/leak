using Leak.Core.Common;

namespace Leak.Core.Metaget
{
    public abstract class MetagetCallbackBase : MetagetCallback
    {
        public virtual void OnMetadataMeasured(PeerHash peer, int size)
        {
        }

        public virtual void OnMetadataRequested(PeerHash peer, int piece)
        {
        }

        public virtual void OnMetadataReceived(PeerHash peer, int piece)
        {
        }

        public virtual void OnMetadataCompleted(FileHash hash)
        {
        }
    }
}