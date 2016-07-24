using Leak.Core.Common;

namespace Leak.Core.Extensions
{
    public interface ExtenderFormattable
    {
        byte Translate(PeerHash peer, string name);
    }
}