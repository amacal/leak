using Leak.Common;
using System.Linq;

namespace Leak.Repository.Tests
{
    public class RepositoryData
    {
        private readonly byte[] bytes;

        public RepositoryData(int size)
        {
            bytes = Bytes.Random(size);
        }

        public int Pieces
        {
            get { return (bytes.Length - 1) / 16384 + 1; }
        }

        public byte[] ToBytes()
        {
            return bytes;
        }

        public byte[] this[int index]
        {
            get { return bytes.Skip(index * 16384).Take(16384).ToArray(); }
        }
    }
}