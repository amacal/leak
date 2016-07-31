namespace Leak.Core.Collector
{
    public class PeerCollectorStatus
    {
        private bool interested;
        private bool unchoked;

        public bool IsInterested()
        {
            return interested;
        }

        public void SetInterested(bool value)
        {
            interested = value;
        }

        public bool IsChoked()
        {
            return unchoked;
        }

        public void SetChoked(bool value)
        {
            unchoked = value == false;
        }
    }
}