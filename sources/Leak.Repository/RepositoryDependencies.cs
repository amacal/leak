using Leak.Common;
using Leak.Files;
using Leak.Tasks;

namespace Leak.Repository
{
    public class RepositoryDependencies
    {
        public FileFactory Files;

        public DataBlockFactory Blocks;

        public LeakPipeline Pipeline;
    }
}
