using Leak.Files;
using Leak.Glue;
using Leak.Metaget;
using Leak.Metashare;
using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanDependencies
    {
        public LeakPipeline Pipeline;

        public GlueService Glue;

        public FileFactory Files;
        
        public MetagetService Metaget;

        public MetashareService Metashare;
    }
}
