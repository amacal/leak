using Leak.Common;

namespace Leak.Client.Swarm
{
    public interface SwarmFilter
    {
        bool Accept(PeerAddress address);
    }
}