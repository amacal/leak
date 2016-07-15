using System;

namespace Leak.Core.Negotiator
{
    [Flags]
    public enum HandshakeOptions : uint
    {
        None = 0,
        Extended = 0x100000
    }
}