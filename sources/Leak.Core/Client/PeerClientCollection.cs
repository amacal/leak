using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public class PeerClientCollection
    {
        private readonly Dictionary<FileHash, PeerClientEntry> byHash;

        public PeerClientCollection()
        {
            byHash = new Dictionary<FileHash, PeerClientEntry>();
        }
    }
}