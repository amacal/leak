using System;

namespace Leak.Loggers
{
    public class ListenerLoggerNormal : ListenerLogger
    {
        protected override void Handle(string name, dynamic payload, Action next)
        {
            switch (name)
            {
                case "listener-started":
                    Console.WriteLine($"{payload.Peer}: listener started on port {payload.Port}");
                    break;
            }

            next.Invoke();
        }
    }
}