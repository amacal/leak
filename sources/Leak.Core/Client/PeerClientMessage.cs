using Leak.Core.Collector;

namespace Leak.Core.Client
{
    public class PeerClientMessage
    {
        private readonly PeerCollectorMessage inner;

        public PeerClientMessage(PeerCollectorMessage inner)
        {
            this.inner = inner;
        }

        public string Type
        {
            get { return inner.Type; }
        }

        public int Size
        {
            get { return inner.Size; }
        }
    }
}