using Leak.Core.Client;

namespace Leak.Loggers
{
    public class BouncerLogger : PeerClientCallbackBase
    {
        public static PeerClientCallback Off()
        {
            return new BouncerLogger();
        }
    }
}