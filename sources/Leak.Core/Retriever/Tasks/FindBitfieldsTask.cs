using Leak.Core.Retriever.Components;
using Leak.Tasks;

namespace Leak.Core.Retriever.Tasks
{
    public class FindBitfieldsTask : LeakTask<RetrieverContext>
    {
        //private static readonly PeerCollectorCriterion[] Criterion =
        //{
        //    new IsBitfieldKnown()
        //};

        public void Execute(RetrieverContext context)
        {
            //    foreach (PeerSession session in context.Collector.GetPeers(Criterion))
            //    {
            //        Bitfield bitfield = context.Collector.GetBitfield(session);

            //        if (bitfield != null)
            //        {
            //            context.Omnibus.Add(session.Peer, bitfield);
            //            context.Collector.SendLocalInterested(session.Peer);
            //        }
            //    }
        }
    }
}