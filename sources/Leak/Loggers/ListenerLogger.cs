using Leak.Core.Client;

namespace Leak.Loggers
{
    public class ListenerLogger : PeerClientCallbackBase
    {
        public static PeerClientCallback Off()
        {
            return new ListenerLogger();
        }

        public static ListenerLogger Normal()
        {
            return new ListenerLoggerNormal();
        }
    }
}