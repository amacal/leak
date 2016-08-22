using Leak.Core.Common;

namespace Leak.Core.Loop
{
    public interface ConnectionLoopHandshake
    {
        PeerSession Session { get; }

        bool HasExtensions { get; }
    }
}