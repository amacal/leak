using System;

namespace Leak.Loggers
{
    public class HashLogger
    {
        public static HashLogger Off()
        {
            return new HashLogger();
        }

        public static HashLogger Normal()
        {
            return new HashLoggerNormal();
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