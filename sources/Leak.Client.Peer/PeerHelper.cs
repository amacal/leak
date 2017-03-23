using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Peer
{
    public static class PeerHelper
    {
        public static void Download(string destination, FileHash hash, PeerAddress remote)
        {
            DownloadAsync(destination, hash, remote, null).Wait();
        }

        public static void Download(string destination, FileHash hash, PeerAddress remote, NotificationCallback callback)
        {
            DownloadAsync(destination, hash, remote, callback).Wait();
        }

        public static async Task DownloadAsync(string destination, FileHash hash, PeerAddress remote, NotificationCallback callback)
        {
            using (PeerClient client = new PeerClient())
            {
                Notification notification;
                PeerSession session = await client.ConnectAsync(hash, remote);

                session.Download(destination);

                while (true)
                {
                    notification = await session.NextAsync();
                    callback?.Invoke(notification);

                    if (notification.Type == NotificationType.DataCompleted)
                        break;
                }
            }
        }
    }
}