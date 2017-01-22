using Leak.Datashare;
using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanDependencies
    {
        public PipelineService Pipeline;

        public SpartanMetaget Metaget;
        public SpartanMetashare Metashare;

        public SpartanRepository Repository;

        public SpartanRetriever Retriever;
        public DatashareService Datashare;
    }
}