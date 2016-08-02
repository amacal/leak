using Leak.Core.Client;
using Leak.Core.Common;
using System;

namespace Leak.Loggers
{
    public class HashLoggerNormal : HashLogger
    {
        public override void OnInitialized(FileHash hash, PeerClientMetainfo summary)
        {
            Console.WriteLine($"{hash}: initialized; completed={summary.Completed}; total={summary.Total}");
        }

        public override void OnCompleted(FileHash hash)
        {
            Console.WriteLine($"{hash}: completed");
        }
    }
}