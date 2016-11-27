namespace Leak.Core.Retriever
{
    public class RetrieverConfiguration
    {
        public RetrieverConfiguration()
        {
            Strategy = RetrieverStrategy.RarestFirst;
        }

        public RetrieverStrategy Strategy { get; set; }
    }
}