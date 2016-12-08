using Leak.Core.Retriever.Components;
using Leak.Tasks;

namespace Leak.Core.Retriever.Tasks
{
    public class VerifyPieceTask : LeakTask<RetrieverContext>
    {
        public void Execute(RetrieverContext context)
        {
            if (context.Omnibus.IsComplete())
            {
                context.Repository.Flush();
                //context.Callback.OnFileCompleted(context.Metainfo.Hash);
            }
        }
    }
}