using System.Linq;
using Leak.Networking.Core;
using Leak.Peer.Coordinator;
using Leak.Peer.Coordinator.Events;

namespace Leak.Extensions.Peers
{
    public class PeersPlugin : MorePlugin
    {
        public static readonly string Name = "ut_pex";

        private readonly PeersHooks hooks;

        public PeersPlugin(PeersHooks hooks)
        {
            this.hooks = hooks;
        }

        public PeersHooks Hooks
        {
            get { return hooks; }
        }

        public void Install(MoreMapping mapping)
        {
            mapping.Add(Name, new PeersHandler(hooks));
        }

        public void HandlePeerConnected(CoordinatorService service, ExtensionListReceived data)
        {
            if (data.Extensions.Contains(Name) == false)
                return;

            service.ForEachPeer((peer, remote, direction) =>
            {
                if (peer.Equals(data.Peer))
                    return;

                if (direction == NetworkDirection.Incoming)
                    return;

                service.SendPeers(data.Peer, remote);
            });
        }
    }
}