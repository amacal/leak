using Leak.Core.Glue;
using Leak.Files;
using Leak.Tasks;

namespace Leak.Core.Spartan
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