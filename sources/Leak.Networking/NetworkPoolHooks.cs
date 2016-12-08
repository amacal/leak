using System;
using Leak.Events;

namespace Leak.Networking
{
    public class NetworkPoolHooks
    {
        public Action<ConnectionAttached> OnConnectionAttached;

        public Action<ConnectionDropped> OnConnectionDropped;

        public Action<ConnectionTerminated> OnConnectionTerminated;
    }
}