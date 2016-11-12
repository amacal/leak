using Leak.Core.Core;

namespace Leak.Core.Cando
{
    public class CandoConfiguration
    {
        public CandoBuilder Extensions { get; set; }

        public CandoCallback Callback { get; set; }

        public LeakBus Bus { get; set; }
    }
}