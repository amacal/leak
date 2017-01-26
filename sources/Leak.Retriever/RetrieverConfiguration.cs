namespace Leak.Dataget
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