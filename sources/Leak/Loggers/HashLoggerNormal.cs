using Leak.Core.Client;
using Leak.Core.Common;
using System;

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

        public override void OnFileInitialized(FileHash hash, PeerClientMetainfo summary)
        {
            Console.WriteLine($"{hash}: initialized; completed={summary.Completed}; total={summary.Total}");
        }

        public override void OnFileStarted(FileHash hash)
        {
            Console.WriteLine($"{hash}: started");
        }

        public override void OnFileCompleted(FileHash hash)
        {
            Console.WriteLine($"{hash}: completed");
        }

        public override void OnAnnounceCompleted(FileHash hash, PeerClientAnnounced announced)
        {
            Console.WriteLine($"{hash}: announced; peers={announced.Peers}");
        }
    }
}