using System;
using Pargos;

namespace Leak.Connect
{
    public class Options
    {
        [Option("--host")]
        public string Host { get; set; }

        [Option("--port")]
        public string Port { get; set; }

        [Option("--hash")]
        public string Hash { get; set; }

        [Option("--analyze"), Presence]
        public bool Analyze { get; set; }

        public bool IsValid()
        {
            int port;

            return Hash?.Length == 40 && Host.Length > 5 &&
                   Int32.TryParse(Port, out port) && port > 0 && port < 65535;
        }
    }
}