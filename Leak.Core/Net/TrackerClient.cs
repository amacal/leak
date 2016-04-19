using System.Net;
using System.Text;

namespace Leak.Core.Net
{
    public class TrackerClient
    {
        private readonly string uri;

        public TrackerClient(string uri)
        {
            this.uri = uri;
        }

        public TrackerResonse Announce(byte[] hash)
        {
            byte[] data;
            StringBuilder request = new StringBuilder();

            request.Append($"{uri}?");
            request.Append($"info_hash={Encode(hash)}&");
            request.Append($"peer_id={Encode(hash)}&");
            request.Append($"port=8080&");
            request.Append($"uploaded=0&");
            request.Append($"downloaded=0&");
            request.Append($"left=0&");
            request.Append($"compact=1&");
            request.Append($"event=started");

            using (WebClient client = new WebClient())
            {
                data = client.DownloadData(request.ToString());
            }

            return new TrackerResonse(data);
        }

        private string Encode(byte[] value)
        {
            StringBuilder builder = new StringBuilder();

            foreach (byte item in value)
            {
                builder.Append('%');
                builder.Append(item.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}