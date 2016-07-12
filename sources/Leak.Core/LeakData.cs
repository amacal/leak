using Leak.Core.IO;
using Leak.Core.Net;

namespace Leak.Core
{
    public class LeakData
    {
        public PeerListener Listener { get; set; }

        public PeerChannelCollection Peers { get; set; }

        public MetainfoRepository Metainfo { get; set; }
    }
}