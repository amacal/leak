using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leak.Client
{
    public static class NotificationExtensions
    {
        public static IEnumerable<Notification> All(this Session session)
        {
            while (true)
            {
                Task<Notification> next = session.NextAsync();
                Notification notification = next.Result;

                yield return notification;
            }
        }
    }
}