namespace Leak.Core.Collector
{
    public class PeerCollectorView
    {
        private readonly PeerCollectorStorage storage;

        public PeerCollectorView(PeerCollectorStorage storage)
        {
            this.storage = storage;
        }
    }
}