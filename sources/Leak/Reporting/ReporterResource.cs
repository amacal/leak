using System.Diagnostics;
using Leak.Client.Notifications;
using Leak.Common;

namespace Leak.Reporting
{
    public class ReporterResource
    {
        private Size buffers;
        private Process process;

        public ReporterResource()
        {
            buffers = new Size(0);
            process = Process.GetCurrentProcess();
        }

        public void Handle(MemorySnapshotNotification notification)
        {
            buffers = notification.Allocation;
        }

        public override string ToString()
        {
            process.Refresh();

            return $"buffers: {buffers}, memory={new Size(process.PrivateMemorySize64)}";
        }
    }
}