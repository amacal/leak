using Leak.Core.Common;

namespace Leak.Core.Battlefield
{
    public interface BattlefieldCallback
    {
        void OnBitfieldChanged(PeerSession session, Bitfield bitfield);
    }
}