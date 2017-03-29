using System;
using Leak.Client;

namespace Leak.Reporting
{
    public class ReporterVerbose : Reporter
    {
        private readonly string command;

        public ReporterVerbose(string command)
        {
            this.command = command;
        }

        public bool Handle(Notification notification)
        {
            Console.WriteLine(notification);

            if (command == "download")
                return notification.Type != NotificationType.DataCompleted;

            return true;
        }
    }
}