using Leak.Networking.Core;
using Leak.Networking.Events;

namespace Leak.Networking
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

        public static void CallConnectionTerminated(this NetworkPoolHooks hooks, NetworkConnection connection)
        {
            hooks.OnConnectionTerminated?.Invoke(new ConnectionTerminated
            {
                Connection = connection,
                Remote = connection.Remote
            });
        }

        public static void CallConnectionEncrypted(this NetworkPoolHooks hooks, NetworkConnection connection)
        {
            hooks.OnConnectionEncrypted?.Invoke(new ConnectionEncrypted
            {
                Connection = connection,
                Remote = connection.Remote
            });
        }

        public static void CallConnectionSent(this NetworkPoolHooks hooks, NetworkConnection connection, int bytes)
        {
            hooks.OnConnectionSent?.Invoke(new ConnectionSent
            {
                Connection = connection,
                Remote = connection.Remote,
                Bytes = bytes
            });
        }

        public static void CallConnectionReceived(this NetworkPoolHooks hooks, NetworkConnection connection, int bytes)
        {
            hooks.OnConnectionReceived?.Invoke(new ConnectionReceived
            {
                Connection = connection,
                Remote = connection.Remote,
                Bytes = bytes
            });
        }
    }
}