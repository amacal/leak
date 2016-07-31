using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Bouncer
{
    public class PeerBouncerStorageEntry
    {
        public PeerBouncerStorageEntry()
        {
            Remotes = new HashSet<string>();
            Peers = new HashSet<PeerHash>();
        }

        public HashSet<string> Remotes { get; set; }

        public HashSet<PeerHash> Peers { get; set; }
    }
}