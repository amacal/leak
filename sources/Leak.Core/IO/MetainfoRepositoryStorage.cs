namespace Leak.Core.IO
{
    public interface MetainfoRepositoryStorage
    {
        MetainfoRepositoryInclude[] Initialize();

        void Complete(MetainfoRepositoryEntry entry);
    }
}