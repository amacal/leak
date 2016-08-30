using Leak.Core.Client;
using Leak.Core.Common;
using System;
using Leak.Core.Client.Events;

namespace Leak.Loggers
{
    public class HashLoggerNormal : HashLogger
    {
        public override void OnFileScheduled(FileHash hash)
        {
            Console.WriteLine($"{hash}: scheduled");
        }

        public override void OnFileDiscovered(FileHash hash)
        {
            Console.WriteLine($"{hash}: discovered");
        }

        public override void OnFileInitialized(FileHash hash, FileInitializedEvent @event)
        {
            Console.WriteLine($"{hash}: initialized; completed={@event.Completed}; total={@event.Total}");
        }

        public override void OnFileStarted(FileHash hash)
        {
            Console.WriteLine($"{hash}: started");
        }

        public override void OnFileCompleted(FileHash hash)
        {
            Console.WriteLine($"{hash}: completed");
        }

        public override void OnAnnounceCompleted(FileHash hash, FileAnnouncedEvent @event)
        {
            Console.WriteLine($"{hash}: announced; peers={@event.Peers}");
        }
    }
}