using Leak.Core.Client;

namespace Leak.Loggers
{
    public class ConnectorLogger : PeerClientCallbackBase
    {
        public static PeerClientCallback Off()
        {
            return new ConnectorLogger();
        }
    }
}