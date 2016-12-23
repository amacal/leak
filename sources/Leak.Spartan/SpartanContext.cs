using Leak.Files;
using Leak.Glue;
using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanContext
    {
        public string Destination;

        public LeakPipeline Pipeline;
        public GlueService Glue;
        public FileFactory Files;
        public LeakQueue<SpartanContext> Queue;

        public SpartanHooks Hooks;
        public SpartanFacts Facts;
    }
}