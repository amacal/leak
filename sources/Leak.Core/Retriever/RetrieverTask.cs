namespace Leak.Core.Retriever
{
    public interface RetrieverTask
    {
        void Execute(RetrieverContext context);
    }
}