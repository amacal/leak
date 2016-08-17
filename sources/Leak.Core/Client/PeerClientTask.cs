using Leak.Core.Common;

namespace Leak.Core.Client
{
    public interface PeerClientTask
    {
        FileHash Hash { get; }

        void Execute(PeerClientContext context);
    }
}