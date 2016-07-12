namespace Leak.Core.IO
{
    public class MetainfoRepositoryStorageMemory : MetainfoRepositoryStorage
    {
        public MetainfoRepositoryInclude[] Initialize()
        {
            return new MetainfoRepositoryInclude[0];
        }

        public void Complete(MetainfoRepositoryEntry entry)
        {
        }
    }
}