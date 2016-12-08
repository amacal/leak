using System;

namespace Leak.Common
{
    [Flags]
    public enum HandshakeOptions : uint
    {
        None = 0,
        Extended = 0x100000
    }
}