using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.Common
{
    public class FileHashCollection : IEnumerable<FileHash>
    {
        private readonly List<FileHash> hashes;

        public FileHashCollection(params FileHash[] items)
        {
            this.hashes = new List<FileHash>(items);
        }

        public IEnumerator<FileHash> GetEnumerator()
        {
            return hashes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}