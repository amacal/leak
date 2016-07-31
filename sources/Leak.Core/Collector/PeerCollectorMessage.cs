namespace Leak.Core.Collector
{
    public class PeerCollectorMessage
    {
        private readonly string type;
        private readonly int size;

        public PeerCollectorMessage(string type, int size)
        {
            this.type = type;
            this.size = size;
        }

        public string Type
        {
            get { return type; }
        }

        public int Size
        {
            get { return size; }
        }
    }
}