using System.Collections.Generic;
using Leak.Extensions;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorConfiguration
    {
        public CoordinatorConfiguration()
        {
            Plugins = new List<MorePlugin>();
            AnnounceBitfield = false;
        }

        public List<MorePlugin> Plugins;
        public bool AnnounceBitfield;
    }
}