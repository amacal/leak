using Leak.Core.Client;
using System;

namespace Leak.Loggers
{
    public class ConnectorLogger : PeerClientCallbackBase
    {
        public static ConnectorLogger Off()
        {
            return new ConnectorLogger();
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