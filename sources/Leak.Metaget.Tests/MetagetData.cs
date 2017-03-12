using System.Linq;

namespace Leak.Meta.Get.Tests
{
    public class MetagetData
    {
        private readonly byte[] bytes;

        public MetagetData(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public int Size
        {
            get { return bytes.Length; }
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