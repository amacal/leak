using Leak.Common;
using Leak.Core.Events;

namespace Leak.Core.Network
{
    public static class NetworkPoolExtensions
    {
        public static void CallConnectionAttached(this NetworkPoolHooks hooks, NetworkConnection connection)
        {
            hooks.OnConnectionAttached?.Invoke(new ConnectionAttached
            {
                Connection = connection,
                Remote = connection.Remote
            });
        }

        public static void CallConnectionDropped(this NetworkPoolHooks hooks, NetworkConnection connection)
        {
            hooks.OnConnectionDropped?.Invoke(new ConnectionDropped
            {
                Connection = connection,
                Remote = connection.Remote
            });
        }

        public static void CallConnectionDropped(this NetworkPoolHooks hooks, NetworkConnection connection, string reason)
        {
            hooks.OnConnectionDropped?.Invoke(new ConnectionDropped
            {
                Reason = reason,
                Connection = connection,
                Remote = connection.Remote
            });
        }

        public static void CallConnectionTerminated(this NetworkPoolHooks hooks, NetworkConnection connection)
        {
            hooks.OnConnectionTerminated?.Invoke(new ConnectionTerminated
            {
                Connection = connection,
                Remote = connection.Remote
            });
        }
    }
}