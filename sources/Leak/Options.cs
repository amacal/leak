using System;
using Pargos;

namespace Leak
{
    public class Options
    {
        [Parameter, At(0)]
        public string Command { get; set; }

        [Option("--trackers")]
        public string[] Trackers { get; set; }

        [Option("--hash")]
        public string Hash { get; set; }

        [Option("--analyze"), Presence]
        public bool Analyze { get; set; }

        [Option("--destination")]
        public string Destination { get; set; }

        [Option("--port")]
        public string Port { get; set; }

        public bool IsValid()
        {
            int port;

            return Hash?.Length == 40 && Trackers?.Length > 0 &&
                   Int32.TryParse(Port, out port) && port > 0 && port < 65535;
        }
    }
}