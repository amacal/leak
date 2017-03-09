using Leak.Networking;
using Leak.Tasks;

namespace Leak.Client.Swarm
{
    public interface SwarmRuntime
    {
        PipelineService Pipeline { get; }

        NetworkPool Network { get; }

        void Start(NetworkPoolHooks hooks);

        void Stop();
    }
}