using System;

namespace Leak.Core.Net
{
    [Flags]
    public enum PeerHandshakeOptions : uint
    {
        None = 0,
        Extended = 0x100000
    }
}