using System;

namespace Leak.Core.Spartan
{
    [Flags]
    public enum SpartanTask
    {
        None = 0,
        All = Discover | Verify | Download,

        Discover = 1,
        Verify = 2,
        Download = 4
    }
}