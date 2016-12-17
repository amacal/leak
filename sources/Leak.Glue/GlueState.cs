using System;

namespace Leak.Glue
{
    [Flags]
    public enum GlueState
    {
        IsLocalInterestedInRemote = 1,
        IsLocalChockingRemote = 2,

        IsRemoteInterestedInLocal = 4,
        IsRemoteChockingLocal = 8,

        None = 0,
        Default = IsLocalChockingRemote | IsRemoteChockingLocal
    }
}