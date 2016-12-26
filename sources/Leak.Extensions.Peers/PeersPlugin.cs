using System;
using System.Linq;
using Leak.Common;
using Leak.Events;
using Leak.Glue;

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

        public void Install(MoreMapping mapping)
        {
            mapping.Add(Name, new PeersHandler(hooks));
        }

        public void HandlePeerConnected(GlueService service, ExtensionListReceived data)
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
