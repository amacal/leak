using Leak.Core.Core;
using Leak.Core.Retriever.Components;

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