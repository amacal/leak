using Leak.Core.Client;
using System;

namespace Leak.Loggers
{
    public class NetworkLogger : PeerClientCallbackBase
    {
        public static NetworkLogger Off()
        {
            return new NetworkLogger();
        }

        public static NetworkLogger Normal()
        {
            return new NetworkLoggerNormal();
        }

        public static NetworkLogger Verbose()
        {
            return new NetworkLoggerVerbose();
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