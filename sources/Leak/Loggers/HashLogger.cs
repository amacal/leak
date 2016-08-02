using Leak.Core.Client;

namespace Leak.Loggers
{
    public class HashLogger : PeerClientCallbackBase
    {
        public static PeerClientCallback Off()
        {
            return new HashLogger();
        }

        public static PeerClientCallback Normal()
        {
            return new HashLoggerNormal();
        }
    }
}