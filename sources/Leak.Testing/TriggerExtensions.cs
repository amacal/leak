using System;

namespace Leak.Testing
{
    public static class TriggerExtensions
    {
        public static Trigger<T> Trigger<T>(this Action<T> test)
        {
            return new Trigger<T>(data => { });
        }

        public static Trigger<T> Trigger<T>(this Action<T> test, Action<T> callback)
        {
            return new Trigger<T>(callback);
        }
    }
}