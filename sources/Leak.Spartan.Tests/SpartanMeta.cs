using Leak.Common;
using System.Linq;

namespace Leak.Spartan.Tests
{
    public class SpartanMeta
    {
        private readonly FileHash hash;
        private readonly byte[] bytes;

        public SpartanMeta(FileHash hash, byte[] bytes)
        {
            this.hash = hash;
            this.bytes = bytes;
        }

        public int Size
        {
            get { return bytes.Length; }
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public byte[] this[int index]
        {
            get { return bytes.Skip(index * 16384).Take(16384).ToArray(); }
        }
    }
}