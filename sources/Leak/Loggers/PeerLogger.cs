using Leak.Core.Client;

namespace Leak.Loggers
{
    public class PeerLogger : PeerClientCallbackBase
    {
        public static PeerClientCallback Off()
        {
            return new PeerLogger();
        }

        public static PeerClientCallback Normal()
        {
            return new PeerLoggerNormal();
        }

        public static PeerClientCallback Verbose()
        {
            return new PeerLoggerVerbose();
        }
    }
}