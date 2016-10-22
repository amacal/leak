using Leak.Suckets;

namespace Leak.Core.Network
{
    public class NetworkPoolConfiguration
    {
        public CompletionWorker Worker { get; set; }

        public NetworkPoolCallback Callback { get; set; }
    }
}