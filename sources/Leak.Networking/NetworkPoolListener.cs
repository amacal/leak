using System;
using Leak.Tasks;

namespace Leak.Networking
{
    public interface NetworkPoolListener
    {
        bool IsAvailable(long id);

        void OnDisconnected(long id);

        void OnException(long id, Exception ex);

        void Schedule(LeakTask<NetworkPool> task);
    }
}