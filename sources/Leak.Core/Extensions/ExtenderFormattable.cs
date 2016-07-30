using Leak.Core.Common;

namespace Leak.Core.Extensions
{
    public interface ExtenderFormattable
    {
        bool Supports(PeerHash peer, string name);

        byte Translate(PeerHash peer, string name);
    }
}