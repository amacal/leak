using Leak.Core.Core;
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

        public LeakBusCallback Subscriber()
        {
            BouncerLogger bounder = Bouncer();
            ConnectorLogger connector = Connector();
            HashLogger hash = Hash();
            ExtensionLogger extension = Extension();
            ListenerLogger listener = Listener();
            NetworkLogger network = Network();
            PeerLogger peer = Peer();

            return (name, payload) =>
            {
                bounder.Handle(name, payload);
                connector.Handle(name, payload);
                hash.Handle(name, payload);
                extension.Handle(name, payload);
                listener.Handle(name, payload);
                network.Handle(name, payload);
                peer.Handle(name, payload);
            };
        }

        public BouncerLogger Bouncer()
        {
            return BouncerLogger.Off();
        }

        public ConnectorLogger Connector()
        {
            return ConnectorLogger.Off();
        }

        public HashLogger Hash()
        {
            switch (Severity(options.LoggingHash))
            {
                case "off":
                    return HashLogger.Off();

                default:
                    return HashLogger.Normal();
            }
        }

        public ExtensionLogger Extension()
        {
            switch (Severity(options.LoggingExtension))
            {
                case "off":
                    return ExtensionLogger.Off();

                case "verbose":
                    return ExtensionLogger.Verbose();

                default:
                    return ExtensionLogger.Normal();
            }
        }

        public ListenerLogger Listener()
        {
            switch (Severity(options.LoggingListener))
            {
                case "off":
                    return ListenerLogger.Off();

                case "verbose":
                    return ListenerLogger.Verbose();

                default:
                    return ListenerLogger.Normal();
            }
        }

        public NetworkLogger Network()
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

        public PeerLogger Peer()
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