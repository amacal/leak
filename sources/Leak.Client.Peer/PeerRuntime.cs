using Leak.Networking;
using Leak.Tasks;

namespace Leak.Client.Peer
{
    public interface PeerRuntime
    {
        PipelineService Pipeline { get; }

        NetworkPool Network { get; }

        void Start(NetworkPoolHooks hooks);

        void Stop();
    }
}