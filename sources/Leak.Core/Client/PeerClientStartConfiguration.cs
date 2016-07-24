using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public class PeerClientStartConfiguration
    {
        public FileHash Hash { get; set; }

        public List<string> Trackers { get; set; }
    }
}