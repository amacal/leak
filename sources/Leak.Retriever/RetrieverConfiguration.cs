namespace Leak.Retriever
{
    public class RetrieverConfiguration
    {
        public RetrieverConfiguration()
        {
            Strategy = "rarest-first";
        }

        public string Strategy { get; set; }
    }
}