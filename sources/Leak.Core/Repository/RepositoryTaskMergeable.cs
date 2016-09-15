using System.Collections.Generic;

namespace Leak.Core.Repository
{
    public interface RepositoryTaskMergeable
    {
        void MergeInto(ICollection<RepositoryBlockData> blocks);
    }
}