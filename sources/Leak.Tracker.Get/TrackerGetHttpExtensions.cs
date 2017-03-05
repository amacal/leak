using System.Text;
using Leak.Common;

namespace Leak.Tracker.Get
{
    public static class TrackerGetHttpExtensions
    {
        public static string Encode(this FileHash hash)
        {
            return Encode(hash.ToBytes());
        }

        public static string Encode(this PeerHash peer)
        {
            return Encode(peer.ToBytes());
        }

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
    }
}