using Leak.Events;

namespace Leak.Leakage
{
    public static class LeakExtensions
    {
        public static void CallListenerStarted(this LeakHooks hooks, ListenerStarted data)
        {
            hooks.OnListenerStarted?.Invoke(data);
        }
    }
}