namespace Leak.Client.Peer
{
    public interface PeerLogger
    {
        void Info(string message);

        void Error(string message);
    }
}