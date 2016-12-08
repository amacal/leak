namespace Leak.Common
{
    public class MetainfoEntry
    {
        private readonly string[] name;
        private readonly long size;

        public MetainfoEntry(string[] name, long size)
        {
            this.name = name;
            this.size = size;
        }

        public MetainfoEntry(string name, long size)
        {
            this.name = new[] { name };
            this.size = size;
        }

        public string[] Name
        {
            get { return name; }
        }

        public long Size
        {
            get { return size; }
        }
    }
}