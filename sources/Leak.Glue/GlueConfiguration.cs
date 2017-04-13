using System.Collections.Generic;
using Leak.Extensions;
using Leak.Peer.Receiver;

namespace Leak.Peer.Coordinator
{
    public class GlueConfiguration
    {
        public GlueConfiguration()
        {
            Plugins = new List<MorePlugin>();
            AnnounceBitfield = false;
        }

        public ReceiverDefinition Definition;
        public List<MorePlugin> Plugins;
        public bool AnnounceBitfield;
    }
}