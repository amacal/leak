using System;

namespace Leak.Core.Net
{
    [Flags]
    public enum PeerStatus
    {
        None,
        Choking,
        Interested
    }
}