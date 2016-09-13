using Leak.Core.Common;

namespace Leak.Core.Battlefield
{
    public abstract class BattlefieldCallbackBase : BattlefieldCallback
    {
        public virtual void OnBitfieldChanged(PeerSession session, Bitfield bitfield)
        {
        }
    }
}