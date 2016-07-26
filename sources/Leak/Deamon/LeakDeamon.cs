using Leak.Core;
using Leak.Core.Client;
using Leak.Core.Common;
using System;
using System.Diagnostics;
using System.Threading;

namespace Leak.Deamon
{
    public class LeakDeamon
    {
        private readonly LeakDeamonConfiguration configuration;

        public LeakDeamon(Action<LeakDeamonConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Identifier = Bytes.Random(20);
            });
        }

        public void Start()
        {
            Process current = Process.GetCurrentProcess();
            string filename = current.MainModule.FileName;

            string directory = configuration.Directory;
            string identifier = Bytes.ToString(configuration.Identifier);

            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = $@"deamon serve --directory ""{directory}"" --identifier {identifier}",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(info);
        }

        public void Stop()
        {
            new Semaphore(0, 1, Bytes.ToString(configuration.Identifier)).Release();
        }

        public void Serve()
        {
            Semaphore mutex = new Semaphore(0, 1, Bytes.ToString(configuration.Identifier));

            LeakMonitor monitor = new LeakMonitor(with =>
            {
                with.Directory = configuration.Directory;
            });

            PeerClient client = new PeerClient(with =>
            {
                with.Peer = new PeerHash(configuration.Identifier);
                with.Destination = configuration.Directory;
                with.Callback = new LeakCallback(monitor);
            });

            monitor.Start(client);
            mutex.WaitOne();
        }
    }
}