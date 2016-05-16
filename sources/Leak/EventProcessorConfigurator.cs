using System;
using System.Collections.Generic;

namespace Leak
{
    public class EventProcessorConfigurator : EventProcessorConfiguration
    {
        private readonly Dictionary<string, Action<dynamic>> handlers;

        public EventProcessorConfigurator()
        {
            this.handlers = new Dictionary<string, Action<dynamic>>();
        }

        public Action<dynamic> GetHandler(string type)
        {
            return handlers[type];
        }

        public void Handle(string type, Action<dynamic> callback)
        {
            handlers.Add(type, callback);
        }
    }
}