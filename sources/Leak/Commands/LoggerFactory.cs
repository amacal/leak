using Leak.Core.Client;
using Leak.Loggers;

namespace Leak.Commands
{
    public class LoggerFactory
    {
        private readonly DownloadOptions options;

        public LoggerFactory(DownloadOptions options)
        {
            this.options = options;
        }

        public PeerClientCallback Bouncer()
        {
            return BouncerLogger.Off();
        }

        public PeerClientCallback Connector()
        {
            return ConnectorLogger.Off();
        }

        public PeerClientCallback Hash()
        {
            switch (Severity(options.LoggingHash))
            {
                case "off":
                    return HashLogger.Off();

                default:
                    return HashLogger.Normal();
            }
        }

        public PeerClientCallback Listener()
        {
            switch (Severity(options.LoggingListener))
            {
                case "off":
                    return ListenerLogger.Off();

                default:
                    return ListenerLogger.Normal();
            }
        }

        public PeerClientCallback Network()
        {
            switch (Severity(options.LoggingNetwork))
            {
                case "off":
                    return NetworkLogger.Off();

                case "verbose":
                    return NetworkLogger.Verbose();

                default:
                    return NetworkLogger.Normal();
            }
        }

        public PeerClientCallback Peer()
        {
            switch (Severity(options.LoggingPeer))
            {
                case "off":
                    return PeerLogger.Off();

                case "verbose":
                    return PeerLogger.Verbose();

                default:
                    return PeerLogger.Normal();
            }
        }

        private string Severity(string value)
        {
            return value ?? options.Logging;
        }
    }
}