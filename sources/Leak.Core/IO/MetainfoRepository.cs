using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Leak.Core.IO
{
    public class MetainfoRepository
    {
        private readonly MetainfoRepositoryConfiguration configuration;
        private readonly List<MetainfoRepositoryEntry> entries;

        public MetainfoRepository(Action<MetainfoRepositoryConfiguration> configurer)
        {
            configuration = new MetainfoRepositoryConfiguration();
            configurer.Invoke(configuration);

            entries = new List<MetainfoRepositoryEntry>();

            Include(entries, configuration.Storage.Initialize());
            Include(entries, configuration.Includes);
        }

        private static void Include(List<MetainfoRepositoryEntry> entries, IEnumerable<MetainfoRepositoryInclude> includes)
        {
            entries.AddRange(includes.Select(ToEntry));
        }

        private static MetainfoRepositoryEntry ToEntry(MetainfoRepositoryInclude include)
        {
            return new MetainfoRepositoryEntry(include.Hash, include.Data);
        }

        public bool Contains(byte[] hash)
        {
            return FindEntry(hash) != null;
        }

        public bool IsCompleted(byte[] hash)
        {
            return FindEntry(hash)?.IsCompleted() == true;
        }

        public void Register(byte[] hash)
        {
            entries.Add(new MetainfoRepositoryEntry(hash));
        }

        public void SetTotalSize(byte[] hash, int size)
        {
            FindEntry(hash)?.SetTotalSize(size);
        }

        public int GetTotalSize(byte[] hash)
        {
            MetainfoRepositoryEntry entry = FindEntry(hash);

            if (entry.IsCompleted() == false)
                return 0;

            return entry.GetTotalSize();
        }

        public byte[] GetData(byte[] hash, int piece)
        {
            MetainfoRepositoryEntry entry = FindEntry(hash);

            if (entry.IsCompleted() == false)
                return null;

            return entry.GetData(piece);
        }

        public bool SetData(byte[] hash, int piece, byte[] data)
        {
            MetainfoRepositoryEntry entry = FindEntry(hash);
            if (entry.IsCompleted())
                return true;

            entry.SetData(piece, data);
            if (entry.IsCompleted() == false)
                return false;

            return Complete(entry);
        }

        private bool Complete(MetainfoRepositoryEntry entry)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                memory.Write("d4:info");
                memory.Write(entry.ToArray());
                memory.Write("e");

                configuration.Storage.Complete(entry);
                configuration.OnCompleted?.Invoke(new MetainfoFile(memory.ToArray()));
            }

            return true;
        }

        private MetainfoRepositoryEntry FindEntry(byte[] hash)
        {
            foreach (MetainfoRepositoryEntry entry in entries)
            {
                if (Bytes.Equals(entry.Hash, hash))
                {
                    return entry;
                }
            }

            return null;
        }
    }
}