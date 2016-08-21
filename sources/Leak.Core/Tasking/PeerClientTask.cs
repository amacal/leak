using Leak.Core.Common;

namespace Leak.Core.Tasking
{
    public interface PeerClientTask
    {
        FileHash Hash { get; }

        PeerClientTaskCallback Start(PeerClientTaskContext context);
    }
}