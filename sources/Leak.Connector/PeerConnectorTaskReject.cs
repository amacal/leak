using Leak.Common;
using Leak.Networking.Core;
using Leak.Tasks;

namespace Leak.Connector
{
    public class PeerConnectorTaskReject : LeakTask<PeerConnectorContext>
    {
        private readonly NetworkAddress remote;

        public PeerConnectorTaskReject(NetworkAddress remote)
        {
            this.remote = remote;
        }

        public void Execute(PeerConnectorContext context)
        {
            context.Hooks.CallConnectionRejected(remote);
        }
    }
}