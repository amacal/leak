using Leak.Core.Core;
using Leak.Core.Retriever.Components;
using Leak.Events;

namespace Leak.Core.Retriever.Tasks
{
    public class HandleBitfieldTask : LeakTask<RetrieverContext>
    {
        private readonly PeerChanged data;

        public HandleBitfieldTask(PeerChanged data)
        {
            this.data = data;
        }

        public void Execute(RetrieverContext context)
        {
            context.Omnibus.Handle(data);
            //context.Collector.SendBitfield(peer, new Bitfield(bitfield.Length));
            //context.Collector.SendLocalInterested(peer);
        }
    }
}