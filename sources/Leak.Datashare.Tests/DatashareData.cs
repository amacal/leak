using Leak.Common;
using System.Linq;

namespace Leak.Datashare.Tests
{
    public class DatashareData
    {
        private readonly byte[] bytes;

        public DatashareData(int size)
        {
            bytes = Bytes.Random(size);
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