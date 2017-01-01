using Leak.Events;
using Leak.Retriever.Components;
using Leak.Tasks;

namespace Leak.Retriever.Tasks
{
    public class RequestBlockTask : LeakTask<RetrieverContext>
    {
        private readonly BlockReserved data;

        public RequestBlockTask(BlockReserved data)
        {
            this.data = data;
        }

        public void Execute(RetrieverContext context)
        {
            context.Dependencies.Glue.SendRequest(data.Peer, data.Piece, data.Block * 16384, data.Size);
            context.Hooks.CallBlockRequested(data.Hash, data.Peer, data.Piece, data.Block);
        }
    }
}