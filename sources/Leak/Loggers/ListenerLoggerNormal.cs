using Leak.Core.Common;
using System;

namespace Leak.Loggers
{
    public class ListenerLoggerNormal : ListenerLogger
    {
        public override void OnListenerStarted(PeerHash local)
        {
            Console.WriteLine($"{local}: listener started");
        }
    }
}