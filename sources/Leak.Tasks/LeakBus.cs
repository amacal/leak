using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Leak.Tasks
{
    public class LeakBus
    {
        private readonly List<Resource> subscribers;
        private readonly Trigger trigger;

        public LeakBus()
        {
            subscribers = new List<Resource>();
            trigger = new Trigger(this);
        }

        public void Start(LeakPipeline pipeline)
        {
            pipeline.Register(trigger);
        }

        public void Publish(string name, object payload)
        {
            trigger.Publish(name, payload);
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

        private class Event
        {
            public string Name;
            public object Payload;
        }

        private class Trigger : LeakPipelineTrigger
        {
            private readonly LeakBus bus;
            private readonly ConcurrentQueue<Event> events;

            private ManualResetEvent onReady;

            public Trigger(LeakBus bus)
            {
                this.bus = bus;
                this.events = new ConcurrentQueue<Event>();
            }

            public void Register(ManualResetEvent watch)
            {
                onReady = watch;
            }

            public void Publish(string name, object payload)
            {
                events.Enqueue(new Event
                {
                    Name = name,
                    Payload = payload
                });

                onReady.Set();
            }

            public void Execute()
            {
                Event data;
                onReady.Reset();

                while (events.TryDequeue(out data))
                {
                    lock (bus.subscribers)
                    {
                        foreach (Resource subscriber in bus.subscribers)
                        {
                            subscriber.Push(data.Name, data.Payload);
                        }
                    }
                }
            }
        }
    }
}