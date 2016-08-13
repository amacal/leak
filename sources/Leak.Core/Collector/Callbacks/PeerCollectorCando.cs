using Leak.Core.Cando;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector.Callbacks
{
    public class PeerCollectorCando : CandoCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorCando(PeerCollectorContext context)
        {
            this.context = context;
        }

        public override void OnOutgoingMessage(PeerHash peer, ExtendedOutgoingMessage message)
        {
            context.Communicator.Get(peer).Send(message);
        }
    }
}