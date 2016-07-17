namespace Leak.Core.Common
{
    public class FileHash
    {
        private readonly byte[] value;

        public FileHash(byte[] value)
        {
            this.value = value;
        }

        public byte[] ToBytes()
        {
            return value;
        }
    }
}