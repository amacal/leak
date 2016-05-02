using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.IO
{
    public class TorrentFileCollection : IEnumerable<TorrentFile>
    {
        private readonly MetainfoFile metainfo;
        private readonly string directory;

        public TorrentFileCollection(MetainfoFile metainfo, string directory)
        {
            this.metainfo = metainfo;
            this.directory = directory;
        }

        public IEnumerator<TorrentFile> GetEnumerator()
        {
            long offset = 0;

            foreach (MetainfoEntry entry in metainfo.Entries)
            {
                yield return new TorrentFile(entry, directory, offset);
                offset += entry.Size;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}