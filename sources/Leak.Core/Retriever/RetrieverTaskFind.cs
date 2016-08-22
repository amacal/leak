using Leak.Core.Collector;
using Leak.Core.Collector.Criterions;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Retriever
{
    public class RetrieverTaskFind : RetrieverTask
    {
        private static readonly PeerCollectorCriterion[] Criterion =
        {
            new IsBitfieldKnown()
        };

        public void Execute(RetrieverContext context)
        {
            foreach (PeerSession session in context.Collector.GetPeers(Criterion))
            {
                Bitfield bitfield = context.Collector.GetBitfield(session.Peer);

                if (bitfield != null)
                {
                    context.Omnibus.Add(session.Peer, bitfield);
                    context.Collector.SendLocalInterested(session.Peer);
                }
            }
        }
    }
}