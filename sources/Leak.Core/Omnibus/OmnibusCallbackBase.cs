using Leak.Core.Common;
using Leak.Core.Omnibus.Events;

namespace Leak.Core.Omnibus
{
    public abstract class OmnibusCallbackBase : OmnibusCallback
    {
        public virtual void OnFileCompleted(FileHash hash)
        {
        }

        public virtual void OnProgressChanged(FileHash hash, BitfieldInfo bitfield)
        {
        }

        public virtual void OnPieceReady(FileHash hash, PieceInfo piece)
        {
        }

        public virtual void OnPieceCompleted(FileHash hash, PieceInfo piece)
        {
        }

        public virtual void OnBlockReserved(FileHash hash, OmnibusReservationEvent @event)
        {
        }

        public virtual void OnBlockExpired(FileHash hash, PeerHash peer, OmnibusBlock block)
        {
        }
    }
}