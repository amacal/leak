using System;
using Leak.Client;

namespace Leak
{
    public class ReporterVerbose : Reporter
    {
        public bool Handle(Notification notification)
        {
            Console.WriteLine(notification);

            return notification.Type != NotificationType.DataCompleted;
        }
    }
}