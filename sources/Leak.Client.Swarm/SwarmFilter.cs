using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Client.Swarm
{
    public interface SwarmFilter
    {
        bool Accept(NetworkAddress address);
    }
}