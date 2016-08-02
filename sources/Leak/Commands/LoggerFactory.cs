using Leak.Core.Client;
using Leak.Loggers;
using Pargos;

namespace Leak.Commands
{
    public class LoggerFactory
    {
        private readonly ArgumentCollection arguments;

        public LoggerFactory(ArgumentCollection arguments)
        {
            this.arguments = arguments;
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
            switch (Severity("hash"))
            {
                case "off":
                    return HashLogger.Off();

                default:
                    return HashLogger.Normal();
            }
        }

        public PeerClientCallback Listener()
        {
            return ListenerLogger.Off();
        }

        public PeerClientCallback Network()
        {
            switch (Severity("network"))
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
            switch (Severity("peer"))
            {
                case "off":
                    return PeerLogger.Off();

                case "verbose":
                    return PeerLogger.Verbose();

                default:
                    return PeerLogger.Normal();
            }
        }

        private string Severity(string component)
        {
            return arguments.GetString($"logging-{component}") ?? arguments.GetString("logging");
        }
    }
}