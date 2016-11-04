using Pargos;

namespace Leak.Commands
{
    public class DownloadOptions
    {
        [Option("--destination")]
        public string Destination { get; set; }

        [Option("--torrent")]
        public string Torrent { get; set; }

        [Option("--hash")]
        public string Hash { get; set; }

        [Option("--tracker")]
        public string[] Tracker { get; set; }

        [Option("--listener")]
        public string Listener { get; set; }

        [Option("--connector")]
        public string Connector { get; set; }

        [Option("--metadata")]
        public string Metadata { get; set; }

        [Option("--peer-exchange")]
        public string PeerExchange { get; set; }

        [Option("--download")]
        public string Download { get; set; }

        [Option("--port")]
        public int? Port { get; set; }

        [Option("--logging")]
        public string Logging { get; set; }

        [Option("--logging-hash")]
        public string LoggingHash { get; set; }

        [Option("--logging-listener")]
        public string LoggingListener { get; set; }

        [Option("--logging-network")]
        public string LoggingNetwork { get; set; }

        [Option("--logging-peer")]
        public string LoggingPeer { get; set; }
    }
}