using Leak.Core.Core;
using Leak.Core.Events;
using Leak.Core.Retriever.Components;

namespace Leak.Core.Retriever.Tasks
{
    public class HandleDataReceived : LeakTask<RetrieverContext>
    {
        private readonly DataReceived data;

        public HandleDataReceived(DataReceived data)
        {
            this.data = data;
        }

        public void Execute(RetrieverContext context)
        {
            if (context.Omnibus.IsComplete(data.Piece) == false)
            {
                context.Repository.Write(data.Piece, data.Block * 16384, data.Payload);
                //context.Collector.Increase(peer, 2);
            }
        }
    }
}