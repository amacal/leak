using Leak.Common;
using Leak.Tasks;

namespace Leak.Core.Connector
{
    public class PeerConnectorTaskReject : LeakTask<PeerConnectorContext>
    {
        private readonly PeerAddress remote;

        public PeerConnectorTaskReject(PeerAddress remote)
        {
            this.remote = remote;
        }

        public void Execute(PeerConnectorContext context)
        {
            context.Hooks.CallConnectionRejected(remote);
        }
    }
}