using System;
using Pargos;

namespace Leak.Announce
{
    public class Options
    {
        [Option("--tracker")]
        public string Tracker { get; set; }

        [Option("--hash")]
        public string[] Hashes { get; set; }

        public bool IsValid()
        {
            return Uri.IsWellFormedUriString(Tracker, UriKind.Absolute) && Hashes?.Length > 0;
        }
    }
}