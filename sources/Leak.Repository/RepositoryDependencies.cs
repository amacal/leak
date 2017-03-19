using Leak.Files;
using Leak.Tasks;

namespace Leak.Data.Store
{
    public class RepositoryDependencies
    {
        public FileFactory Files;

        public RepositoryMemory Memory;

        public PipelineService Pipeline;
    }
}