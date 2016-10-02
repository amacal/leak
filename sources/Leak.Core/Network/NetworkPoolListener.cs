using Leak.Core.Core;
using System;

namespace Leak.Core.Network
{
    public interface NetworkPoolListener
    {
        bool IsAvailable(long id);

        void OnDisconnected(long id);

        void OnException(long id, Exception ex);

        void Schedule(LeakTask<NetworkPool> task);
    }
}