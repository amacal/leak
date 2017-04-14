using System.Collections.Generic;
using Leak.Extensions;
using Leak.Peer.Receiver;
using Leak.Peer.Sender;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorConfiguration
    {
        public CoordinatorConfiguration()
        {
            Plugins = new List<MorePlugin>();
            AnnounceBitfield = false;
        }

        public ReceiverDefinition ReceiverDefinition;
        public SenderDefinition SenderDefinition;

        public List<MorePlugin> Plugins;
        public bool AnnounceBitfield;
    }
}