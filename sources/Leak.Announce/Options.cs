using System;
using Pargos;

namespace Leak.Announce
{
    public class Options
    {
        [Option("--tracker")]
        public string Tracker { get; set; }

        [Option("--hash")]
        public string[] Hash { get; set; }

        [Option("--analyze"), Presence]
        public bool Analyze { get; set; }

        public bool IsValid()
        {
            return Uri.IsWellFormedUriString(Tracker, UriKind.Absolute) && Hash?.Length > 0;
        }
    }
}