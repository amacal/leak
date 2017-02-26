namespace Leak.Common
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

        public byte this[int index]
        {
            get { return value[index]; }
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            FileHash other = obj as FileHash;

            return other != null && Bytes.Equals(other.value, value);
        }

        public override string ToString()
        {
            return Bytes.ToString(value);
        }

        public static FileHash Parse(string data)
        {
            return new FileHash(Bytes.Parse(data));
        }

        public static FileHash Random()
        {
            return new FileHash(Bytes.Random(20));
        }
    }
}