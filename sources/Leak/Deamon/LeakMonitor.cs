using Leak.Core;
using Leak.Core.Client;
using System;
using System.IO;

namespace Leak.Deamon
{
    public class LeakMonitor
    {
        private readonly LeakMonitorConfiguration configuration;

        public LeakMonitor(Action<LeakMonitorConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
            });
        }

        public void Start(PeerClient client)
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = configuration.Directory,
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.CreationTime,
                Filter = "*.metainfo",
                EnableRaisingEvents = true
            };
        }
    }
}