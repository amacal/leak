using System;

namespace Leak.Core.Retriever
{
    [Flags]
    public enum ResourcePeerStatus
    {
        None = 0,
        Unchoke = 1
    }
}