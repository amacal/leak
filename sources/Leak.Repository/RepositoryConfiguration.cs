namespace Leak.Data.Store
{
    public class RepositoryConfiguration
    {
        public RepositoryConfiguration()
        {
            BufferSize = 32 * 1024;
        }

        public int BufferSize;
    }
}