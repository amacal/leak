using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Retriever.Components;

namespace Leak.Core.Retriever.Tasks
{
    public class HandleBitfieldTask : LeakTask<RetrieverContext>
    {
        private readonly PeerHash peer;
        private readonly Bitfield bitfield;

        public HandleBitfieldTask(PeerHash peer, Bitfield bitfield)
        {
            this.peer = peer;
            this.bitfield = bitfield;
        }

        public void Execute(RetrieverContext context)
        {
            context.Omnibus.Add(peer, bitfield);
            //context.Collector.SendBitfield(peer, new Bitfield(bitfield.Length));
            //context.Collector.SendLocalInterested(peer);
        }
    }
}