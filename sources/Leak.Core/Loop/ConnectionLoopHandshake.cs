using Leak.Core.Common;

namespace Leak.Core.Loop
{
    public interface ConnectionLoopHandshake
    {
        PeerHash Peer { get; }

        FileHash Hash { get; }

        bool HasExtensions { get; }
    }
}