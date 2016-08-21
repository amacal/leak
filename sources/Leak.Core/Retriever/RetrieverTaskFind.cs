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
            foreach (PeerHash peer in context.Collector.GetPeers(Criterion))
            {
                Bitfield bitfield = context.Collector.GetBitfield(peer);

                if (bitfield != null)
                {
                    context.Omnibus.Add(peer, bitfield);
                    context.Collector.SendLocalInterested(peer);
                }
            }
        }
    }
}