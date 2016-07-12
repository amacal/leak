using System;

namespace Leak.Core.IO
{
    public static class MetainfoRepositoryExtensions
    {
        public static void Directory(this MetainfoRepositoryConfiguration configuration, string location)
        {
            configuration.Directory(with =>
            {
                with.Location = location;
            });
        }

        public static void Directory(this MetainfoRepositoryConfiguration configuration, Action<MetainfoRepositoryStorageDirectoryConfiguration> configurer)
        {
            configuration.Storage = new MetainfoRepositoryStorageDirectory(configurer);
        }

        public static void Memory(this MetainfoRepositoryConfiguration configuration)
        {
            configuration.Storage = new MetainfoRepositoryStorageMemory();
        }

        public static void Include(this MetainfoRepositoryConfiguration configuration, MetainfoFile metainfo)
        {
            configuration.Includes.Add(new MetainfoRepositoryInclude
            {
                Hash = metainfo.Hash,
                Data = metainfo.Data
            });
        }

        public static void Include(this MetainfoRepositoryConfiguration configuration, byte[] hash)
        {
            configuration.Includes.Add(new MetainfoRepositoryInclude
            {
                Hash = hash
            });
        }
    }
}