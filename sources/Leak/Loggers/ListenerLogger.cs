using System;

namespace Leak.Loggers
{
    public class ListenerLogger
    {
        public static ListenerLogger Off()
        {
            return new ListenerLogger();
        }

        public static ListenerLogger Normal()
        {
            return new ListenerLoggerNormal();
        }

        public void Handle(string name, dynamic payload)
        {
            Handle(name, payload, new Action(() => { }));
        }

        protected virtual void Handle(string name, dynamic payload, Action next)
        {
        }
    }
}