namespace Leak.Core.Net
{
    public abstract class PeerChannel
    {
        public abstract PeerDescription Description { get; }

        public abstract void Send(PeerMessageFactory data);
    }
}