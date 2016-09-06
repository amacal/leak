using Leak.Core.Collector;
using Leak.Core.Collector.Criterions;
using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Retriever.Components;

namespace Leak.Core.Retriever.Tasks
{
    public class FindBitfieldsTask : LeakTask<RetrieverContext>
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