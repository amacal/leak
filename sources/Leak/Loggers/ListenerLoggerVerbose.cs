using System;

namespace Leak.Loggers
{
    public class ListenerLoggerVerbose : ListenerLoggerNormal
    {
        public override void Handle(string name, object payload)
        {
            HandleMessage(name, payload);
            base.Handle(name, payload);
        }

        private void HandleMessage(string name, dynamic payload)
        {
            switch (name)
            {
                case "listener-accepting":
                    Console.WriteLine($"{payload.Local}: accepting {payload.Remote}");
                    break;

                case "listener-accepted":
                    Console.WriteLine($"{payload.Local}: accepted {payload.Remote}");
                    break;
            }
        }
    }
}