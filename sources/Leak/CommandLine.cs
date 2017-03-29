using System;
using Leak.Client.Swarm;
using Leak.Options;
using Leak.Reporting;
using Pargos;

namespace Leak
{
    public class CommandLine
    {
        [Parameter, At(0)]
        public string Command { get; set; }

        [Option("--trackers")]
        public string[] Trackers { get; set; }

        [Option("--hash")]
        public string Hash { get; set; }

        [Option("--destination")]
        public string Destination { get; set; }

        [Option("--listener")]
        public string Listener { get; set; }

        [Option("--port")]
        public string Port { get; set; }

        [Option("--connector")]
        public string Connector { get; set; }

        [Option("--accept")]
        public string[] Accept { get; set; }

        [Option("--strategy")]
        public string Strategy { get; set; }

        [Option("--metadata")]
        public string Metadata { get; set; }

        [Option("--exchange")]
        public string Exchange { get; set; }

        [Option("--logging")]
        public string Logging { get; set; }

        public bool IsValid()
        {
            if (Logging != null)
            {
                switch (Logging)
                {
                    case "compact":
                    case "verbose":
                        break;

                    default:
                        return false;
                }
            }

            switch (Command)
            {
                case "download":
                    return DownloadOption.IsValid(this);

                case "seed":
                    return SeedOption.IsValid(this);
            }

            return false;
        }

        public Reporter ToReporter()
        {
            switch (Logging)
            {
                case null:
                case "compact":
                    return new ReporterCompact(Command);

                default:
                    return new ReporterVerbose(Command);
            }
        }

        public SwarmSettings ToSettings()
        {
            SwarmSettings settings = new SwarmSettings();

            if (Connector != null)
            {
                settings.Connector = Connector == "on";
            }

            if (Listener != null)
            {
                settings.Listener = Listener == "on";
            }

            if (settings.Listener && Port != null)
            {
                settings.ListenerPort = Int32.Parse(Port);
            }

            if (Accept != null)
            {
                settings.Filter = new GeonFilter(Accept);
            }

            if (Strategy != null)
            {
                settings.Strategy = Strategy;
            }

            if (Metadata != null)
            {
                settings.Metadata = Metadata == "on";
            }

            if (Exchange != null)
            {
                settings.Exchange = Exchange == "on";
            }

            return settings;
        }
    }
}