using System;
using System.IO;
using Leak.Client;
using Leak.Common;

namespace Leak
{
    public class ReporterNormal : Reporter
    {
        public bool Handle(Notification notification)
        {
            switch (notification.Type)
            {
                case NotificationType.MetafileCompleted:

                    break;
            }

            return true;
        }
    }
}