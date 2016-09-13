using Leak.Core.Battlefield;
using Leak.Core.Common;

namespace Leak.Core.Collector.Callbacks
{
    public class PeerCollectorToBattlefield : BattlefieldCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorToBattlefield(PeerCollectorContext context)
        {
            this.context = context;
        }

        public override void OnBitfieldChanged(PeerSession session, Bitfield bitfield)
        {
            context.Callback.OnBitfield(session, bitfield);
        }
    }
}