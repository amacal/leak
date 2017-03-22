using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Swarm
{
    public static class SwarmHelper
    {
        public static void Download(string destination, FileHash hash, string tracker)
        {
            DownloadAsync(destination, hash, tracker, null).Wait();
        }

        public static void Download(string destination, FileHash hash, string tracker, NotificationCallback callback)
        {
            DownloadAsync(destination, hash, tracker, callback).Wait();
        }

        public static async Task DownloadAsync(string destination, FileHash hash, string tracker, NotificationCallback callback)
        {
            using (SwarmClient client = new SwarmClient())
            {
                Notification notification;
                SwarmSession session = await client.ConnectAsync(hash, tracker);

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