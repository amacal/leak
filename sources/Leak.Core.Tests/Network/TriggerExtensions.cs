using System;

namespace Leak.Core.Tests.Network
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