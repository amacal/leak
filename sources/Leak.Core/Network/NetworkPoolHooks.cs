using Leak.Core.Events;
using System;

namespace Leak.Core.Network
{
    public class NetworkPoolHooks
    {
        public Action<ConnectionAttached> OnConnectionAttached;

        public Action<ConnectionDropped> OnConnectionDropped;

        public Action<ConnectionTerminated> OnConnectionTerminated;
    }
}