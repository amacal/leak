using System;
using System.Threading;

namespace Leak.Core.Tests.Network
{
    public class Trigger<T>
    {
        private readonly Action<T> callback;
        private bool called;

        public Trigger(Action<T> callback)
        {
            this.callback = payload =>
            {
                callback.Invoke(payload);
                called = true;
            };
        }

        public bool Complete()
        {
            for (int i = 0; i < 500; i++)
            {
                if (called == false)
                {
                    Thread.Sleep(10);
                }
            }

            return called;
        }

        public static implicit operator Action<T>(Trigger<T> trigger)
        {
            return trigger.callback;
        }
    }
}