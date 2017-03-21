namespace Leak.Data.Get
{
    public class DataGetConfiguration
    {
        public DataGetConfiguration()
        {
            Strategy = "rarest-first";
        }

        public string Strategy { get; set; }
    }
}