using Leak.Events;
using Leak.Retriever.Components;
using Leak.Tasks;

namespace Leak.Retriever.Tasks
{
    public class HandleBlockReceived : LeakTask<RetrieverContext>
    {
        private readonly BlockReceived data;

        public HandleBlockReceived(BlockReceived data)
        {
            this.data = data;
        }

        public void Execute(RetrieverContext context)
        {
            if (context.Dependencies.Omnibus.IsComplete(data.Block.Piece) == false)
            {
                context.Dependencies.Repository.Write(data.Block, data.Payload);
                context.Hooks.CallBlockHandled(data.Hash, data.Peer, data.Block);
                //context.Collector.Increase(peer, 2);
            }
        }
    }
}