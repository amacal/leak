using Leak.Core.Client;
using System;

namespace Leak.Loggers
{
    public class BouncerLogger : PeerClientCallbackBase
    {
        public static BouncerLogger Off()
        {
            return new BouncerLogger();
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