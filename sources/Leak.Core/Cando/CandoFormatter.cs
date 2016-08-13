namespace Leak.Core.Cando
{
    public class CandoFormatter
    {
        private readonly CandoMap map;

        public CandoFormatter(CandoMap map)
        {
            this.map = map;
        }

        public byte Translate(string extension)
        {
            return map.Translate(extension);
        }
    }
}