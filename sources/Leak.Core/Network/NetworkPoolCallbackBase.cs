using System;

namespace Leak.Core.Network
{
    public abstract class NetworkPoolCallbackBase : NetworkPoolCallback
    {
        public virtual void OnAttached(NetworkConnection connection)
        {
        }

        public virtual void OnDisconnected(NetworkConnection connection)
        {
        }

        public virtual void OnException(NetworkConnection connection, Exception ex)
        {
        }
    }
}