namespace Leak.Core.Net
{
    public interface PeerChannel
    {
        string Name { get; }

        void Send(PeerMessageFactory data);
    }
}