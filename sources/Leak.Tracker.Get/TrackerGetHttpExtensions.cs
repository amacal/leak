using System.Text;
using Leak.Common;

namespace Leak.Tracker.Get
{
    public static class TrackerGetHttpExtensions
    {
        public static string Encode(byte[] data)
        {
            StringBuilder builder = new StringBuilder();

            foreach (byte item in data)
            {
                builder.Append('%');
                builder.Append(item.ToString("x2"));
            }

            return builder.ToString();
        }

        public static void AppendHash(this StringBuilder builder, FileHash hash)
        {
            builder.Append("?info_hash=");
            builder.Append(Encode(hash.ToBytes()));
        }

        public static void AppendPeer(this StringBuilder builder, PeerHash peer)
        {
            builder.Append("&peer_id=");
            builder.Append(Encode(peer.ToBytes()));
        }

        public static void AppendPort(this StringBuilder builder, TrackerGetRegistrant request)
        {
            if (request.Port > 0 && request.Port < 65535)
            {
                builder.Append("&port=");
                builder.Append(request.Port);
            }
        }

        public static void AppendEvent(this StringBuilder builder, TrackerGetRegistrant request)
        {
            switch (request.Event)
            {
                case TrackerGetEvent.Started:
                    builder.Append("&event=started");
                    break;

                case TrackerGetEvent.Stopped:
                    builder.Append("&event=stopped");
                    break;

                case TrackerGetEvent.Completed:
                    builder.Append("&event=completed");
                    break;
            }
        }

        public static void AppendProgress(this StringBuilder builder, TrackerGetRegistrant request)
        {
            if (request.Progress?.Downloaded >= 0)
            {
                builder.Append("&downloaded=");
                builder.Append(request.Progress.Downloaded);
            }

            if (request.Progress?.Uploaded >= 0)
            {
                builder.Append("&uploaded=");
                builder.Append(request.Progress.Uploaded);
            }

            if (request.Progress?.Left >= 0)
            {
                builder.Append("&left=");
                builder.Append(request.Progress.Left);
            }
        }
    }
}