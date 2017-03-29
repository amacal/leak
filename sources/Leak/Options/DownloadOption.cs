using System;
using System.IO;
using System.Linq;

namespace Leak.Options
{
    public static class DownloadOption
    {
        public static bool IsValid(CommandLine option)
        {
            Uri uri;
            int port;

            if (option.Trackers != null)
            {
                if (option.Trackers.Length == 0)
                    return false;

                if (option.Trackers.All(x => Uri.TryCreate(x, UriKind.Absolute, out uri)) == false)
                    return false;
            }

            if (option.Trackers == null)
                return false;

            if (option.Hash != null)
            {
                if (option.Hash?.Length != 40)
                    return false;
            }

            if (option.Hash == null)
                return false;

            if (option.Destination != null)
            {
                if (option.Destination == null || Path.IsPathRooted(option.Destination) == false)
                    return false;

                if (Directory.Exists(option.Destination) == false)
                    return false;
            }

            if (option.Destination == null)
                return false;

            if (option.Port != null)
            {
                if (Int32.TryParse(option.Port, out port) == false)
                    return false;

                if (port <= 0 || port > 65535)
                    return false;
            }

            if (option.Listener != null)
            {
                if (option.Listener != "on" && option.Listener != "off")
                    return false;
            }

            if (option.Connector != null)
            {
                if (option.Connector != "on" && option.Connector != "off")
                    return false;
            }

            if (option.Metadata != null)
            {
                if (option.Metadata != "on" && option.Metadata != "off")
                    return false;
            }

            if (option.Exchange != null)
            {
                if (option.Exchange != "on" && option.Exchange != "off")
                    return false;
            }

            if (option.Accept != null)
            {
                if (option.Accept.Length == 0)
                    return false;

                if (option.Accept.Any(x => x.Length != 2))
                    return false;
            }

            if (option.Strategy != null)
            {
                switch (option.Strategy)
                {
                    case "sequential":
                    case "rarest-first":
                        break;

                    default:
                        return false;
                }
            }

            return true;
        }
    }
}