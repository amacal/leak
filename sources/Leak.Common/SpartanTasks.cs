using System;

namespace Leak.Common
{
    [Flags]
    public enum SpartanTasks
    {
        None = 0,
        All = Discover | Verify | Download,

        Discover = 1,
        Verify = 2,
        Download = 4
    }
}