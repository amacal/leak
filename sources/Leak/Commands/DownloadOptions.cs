using Pargos.Attributes;

namespace Leak.Commands
{
    public class DownloadOptions
    {
        [NamedOption("destination")]
        public string Destination { get; set; }

        [NamedOption("torrent")]
        public string Torrent { get; set; }

        [NamedOption("hash")]
        public string Hash { get; set; }

        [NamedOption("tracker")]
        public string[] Tracker { get; set; }

        [NamedOption("listener")]
        public string Listener { get; set; }

        [NamedOption("connector")]
        public string Connector { get; set; }

        [NamedOption("metadata")]
        public string Metadata { get; set; }

        [NamedOption("peer-exchange")]
        public string PeerExchange { get; set; }

        [NamedOption("port")]
        public int Port { get; set; }

        [NamedOption("logging")]
        public string Logging { get; set; }

        [NamedOption("logging-hash")]
        public string LoggingHash { get; set; }

        [NamedOption("logging-listener")]
        public string LoggingListener { get; set; }

        [NamedOption("logging-network")]
        public string LoggingNetwork { get; set; }

        [NamedOption("logging-peer")]
        public string LoggingPeer { get; set; }
    }
}