namespace Leak.Core.Retriever
{
    public class RetrieverTaskVerify : RetrieverTask
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