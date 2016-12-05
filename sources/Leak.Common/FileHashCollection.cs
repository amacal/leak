using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Common
{
    public class FileHashCollection : IEnumerable<FileHash>
    {
        private readonly List<FileHash> hashes;

        public FileHashCollection(params FileHash[] items)
        {
            this.hashes = new List<FileHash>(items);
        }

        public void Add(FileHash hash)
        {
            lock (hashes)
            {
                hashes.Add(hash);
            }
        }

        public IEnumerator<FileHash> GetEnumerator()
        {
            lock (hashes)
            {
                return hashes.ToArray().OfType<FileHash>().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}