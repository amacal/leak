using System;
using System.Net;
using System.Text;

namespace Leak.Core.Net
{
    public class HttpTrackerClient : TrackerClient
    {
        private readonly Uri uri;

        public HttpTrackerClient(Uri uri)
        {
            this.uri = uri;
        }

        public override TrackerResonse Announce(PeerHandshake handshake)
        {
            byte[] data;
            StringBuilder request = new StringBuilder();

            string hash = Encode(handshake.Hash);
            string peer = Encode(handshake.Peer);

            request.Append($"{uri}?");
            request.Append($"info_hash={hash}&");
            request.Append($"peer_id={peer}&");
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

            return new HttpTrackerResonse(data);
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