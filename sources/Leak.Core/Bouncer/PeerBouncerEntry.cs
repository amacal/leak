using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Bouncer
{
    public class PeerBouncerEntry
    {
        public PeerBouncerEntry()
        {
            Identifiers = new HashSet<long>();
            Remotes = new HashSet<string>();
            Peers = new HashSet<PeerHash>();
        }

        public HashSet<long> Identifiers { get; set; }

        public HashSet<string> Remotes { get; set; }

        public HashSet<PeerHash> Peers { get; set; }

        public bool Released { get; set; }
    }
}