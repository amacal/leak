using Leak.Core.Core;
using System;

namespace Leak.Core.Network
{
    public interface NetworkPoolListener
    {
        bool IsAvailable(long id);

        void OnDisconnected(long id);

        void OnException(long id, Exception ex);

        void OnSend(long id, byte[] data);

        void Schedule(LeakTask<NetworkPool> task);
    }
}