using Leak.Core.Cando;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector.Callbacks
{
    public class PeerCollectorToCando : CandoCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorToCando(PeerCollectorContext context)
        {
            this.context = context;
        }

        public override void OnOutgoingMessage(PeerHash peer, ExtendedOutgoingMessage message)
        {
            context.Communicator.Get(peer).Send(message);
        }
    }
}