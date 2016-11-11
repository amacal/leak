using System.Collections.Generic;

namespace Leak.Core.Core
{
    public class LeakBus
    {
        private readonly List<Resource> subscribers;

        public LeakBus()
        {
            subscribers = new List<Resource>();
        }

        public void Publish(string name, object payload)
        {
            lock (subscribers)
            {
                foreach (Resource subscriber in subscribers)
                {
                    subscriber.Push(name, payload);
                }
            }
        }

        public LeakBusResource Subscribe(LeakBusCallback callback)
        {
            Resource resource = new Resource(this, callback);

            lock (subscribers)
            {
                subscribers.Add(resource);
            }

            return resource;
        }

        private class Resource : LeakBusResource
        {
            private readonly LeakBus bus;
            private readonly LeakBusCallback callback;

            public Resource(LeakBus bus, LeakBusCallback callback)
            {
                this.bus = bus;
                this.callback = callback;
            }

            public void Push(string name, object payload)
            {
                callback.Invoke(name, payload);
            }

            public void Detach()
            {
                lock (this)
                {
                    bus.subscribers.Remove(this);
                }
            }
        }
    }
}