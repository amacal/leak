using System;

namespace Leak.Loggers
{
    public class ExtensionLoggerVerbose : ExtensionLoggerNormal
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
                case "extension-exchanged":
                    Console.WriteLine($"{payload.Peer}: extensions exchanged; supported: {String.Join(", ", payload.Extensions)}");
                    break;

                case "metadata-size-received":
                    Console.WriteLine($"{payload.Peer}: metadata size received; size={payload.Size}");
                    break;
            }
        }
    }
}