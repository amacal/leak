using System.IO;

namespace Leak.Core.IO
{
    public static class MetainfoRepositoryStorageDirectoryExtensions
    {
        public static void Include(this MetainfoRepositoryStorageDirectoryConfiguration configuration, string pattern)
        {
            configuration.Includes = Directory.GetFiles(configuration.Location, pattern, SearchOption.TopDirectoryOnly);
        }
    }
}