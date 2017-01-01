using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanContext
    {
        public SpartanParameters Parameters;
        public SpartanDependencies Dependencies;
        public SpartanConfiguration Configuration;
        public SpartanHooks Hooks;

        public LeakQueue<SpartanContext> Queue;
        public SpartanState State;
    }
}