using Leak.Core.Client;
using System;

namespace Leak.Loggers
{
    public class PeerLogger : PeerClientCallbackBase
    {
        public static PeerLogger Off()
        {
            return new PeerLogger();
        }

        public static PeerLogger Normal()
        {
            return new PeerLoggerNormal();
        }

        public static PeerLogger Verbose()
        {
            return new PeerLoggerVerbose();
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