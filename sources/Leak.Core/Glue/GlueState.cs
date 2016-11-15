using System;

namespace Leak.Core.Glue
{
    [Flags]
    public enum GlueState
    {
        None = 0,
        Default = IsLocalChockingRemote | IsRemoteInterestedInLocal,

        IsLocalInterestedInRemote = 1,
        IsLocalChockingRemote = 2,

        IsRemoteInterestedInLocal = 4,
        IsRemoteChockingLocal = 8
    }
}