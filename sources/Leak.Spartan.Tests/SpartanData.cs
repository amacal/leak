using System.Linq;
using Leak.Common;

namespace Leak.Spartan.Tests
{
    public class SpartanData
    {
        private readonly byte[] bytes;

        public SpartanData(int size)
        {
            this.bytes = Bytes.Random(size);
        }

        public int Count
        {
            get { return (bytes.Length - 1) / 16384 + 1; }
        }

        public byte[] this[int index]
        {
            get { return bytes.Skip(index * 16384).Take(16384).ToArray(); }
        }

        public byte[] ToBytes()
        {
            return bytes;
        }
    }
}
