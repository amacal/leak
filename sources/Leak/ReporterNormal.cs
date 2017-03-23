using System;
using Leak.Client;

namespace Leak
{
    public class ReporterNormal : Reporter
    {
        public bool Handle(Notification notification)
        {
            Console.Write(notification);

            return notification.Type != NotificationType.DataCompleted;
        }
    }
}