using Leak.Common;
using Leak.Files;
using Leak.Tasks;

namespace Leak.Datastore
{
    public class RepositoryDependencies
    {
        public FileFactory Files;

        public DataBlockFactory Blocks;

        public PipelineService Pipeline;
    }
}