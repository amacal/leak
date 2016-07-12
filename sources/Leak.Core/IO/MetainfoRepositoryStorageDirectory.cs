using System;
using System.Collections.Generic;
using System.IO;

namespace Leak.Core.IO
{
    public class MetainfoRepositoryStorageDirectory : MetainfoRepositoryStorage
    {
        private readonly MetainfoRepositoryStorageDirectoryConfiguration configuration;

        public MetainfoRepositoryStorageDirectory(Action<MetainfoRepositoryStorageDirectoryConfiguration> configurer)
        {
            configuration = new MetainfoRepositoryStorageDirectoryConfiguration();
            configurer.Invoke(configuration);
        }

        public void Complete(MetainfoRepositoryEntry entry)
        {
            string path = Path.Combine(configuration.Location, Bytes.ToString(entry.Hash) + ".torrent");
            using (FileStream file = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                file.Write("d4:info");
                file.Write(entry.ToArray());
                file.Write("e");
            }
        }

        public MetainfoRepositoryInclude[] Initialize()
        {
            List<MetainfoRepositoryInclude> entries = new List<MetainfoRepositoryInclude>();

            if (configuration.Includes != null)
            {
                foreach (string include in configuration.Includes)
                {
                    byte[] content = File.ReadAllBytes(include);
                    MetainfoFile file = new MetainfoFile(content);

                    entries.Add(new MetainfoRepositoryInclude
                    {
                        Hash = file.Hash,
                        Data = file.Data
                    });
                }
            }

            return entries.ToArray();
        }
    }
}