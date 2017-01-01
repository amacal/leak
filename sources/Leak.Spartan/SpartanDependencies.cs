using Leak.Files;
using Leak.Glue;
using Leak.Metaget;
using Leak.Metashare;
using Leak.Repository;
using Leak.Retriever;
using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanDependencies
    {
        public LeakPipeline Pipeline;
        public FileFactory Files;

        public GlueService Glue;

        public MetagetService Metaget;
        public MetashareService Metashare;

        public RepositoryService Repository;
        public RetrieverService Retriever;
    }
}
