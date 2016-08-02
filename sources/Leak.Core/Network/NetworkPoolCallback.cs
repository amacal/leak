using System;

namespace Leak.Core.Network
{
    public interface NetworkPoolCallback
    {
        void OnAttached(NetworkConnection connection);

        void OnDisconnected(NetworkConnection connection);

        void OnException(NetworkConnection connection, Exception ex);
    }
}