using Leak.Core.Client;

namespace Leak.Loggers
{
    public class NetworkLogger : PeerClientCallbackBase
    {
        public static PeerClientCallback Off()
        {
            return new NetworkLogger();
        }

        public static PeerClientCallback Normal()
        {
            return new NetworkLoggerNormal();
        }

        public static PeerClientCallback Verbose()
        {
            return new NetworkLoggerVerbose();
        }
    }
}