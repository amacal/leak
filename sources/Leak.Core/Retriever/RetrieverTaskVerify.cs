using Leak.Core.Core;

namespace Leak.Core.Retriever
{
    public class RetrieverTaskVerify : LeakTask<RetrieverContext>
    {
        public void Execute(RetrieverContext context)
        {
            if (context.Omnibus.IsComplete())
            {
                context.Callback.OnCompleted(context.Metainfo.Hash);
            }
        }
    }
}