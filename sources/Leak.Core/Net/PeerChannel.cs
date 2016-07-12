using Leak.Core.Network;

namespace Leak.Core.Net
{
    public interface PeerChannel
    {
        byte[] Hash { get; }

        string Name { get; }

        NetworkConnectionDirection Direction { get; }

        void Send(PeerMessageFactory data);
    }
}