using Leak.Core.Client;
using Leak.Core.Common;
using Leak.Core.Metadata;
using System.Threading;

namespace Leak.Commands
{
    public class DownloadCommand
    {
        private readonly DownloadOptions options;

        public DownloadCommand(DownloadOptions options)
        {
            this.options = options;
        }

        public void Execute()
        {
            ManualResetEvent handle = new ManualResetEvent(false);
            CompositeCallback callback = new CompositeCallback();
            LoggerFactory factory = new LoggerFactory(options);

            callback.Add(factory.Bouncer());
            callback.Add(factory.Connector());
            callback.Add(factory.Hash());
            callback.Add(factory.Listener());
            callback.Add(factory.Network());
            callback.Add(factory.Peer());

            callback.Add(new ReadyCallback(handle));

            PeerClient client = new PeerClient(with =>
            {
                with.Destination = options.Destination;
                with.Callback = callback;

                if (options.Metadata != "off")
                {
                    with.Metadata.Enable();
                }

                if (options.PeerExchange != "off")
                {
                    with.PeerExchange.Enable();
                }

                if (options.Listener == "on")
                {
                    with.Listener.Enable(listener =>
                    {
                        if (options.Port != null && options.Port > 0 && options.Port < 65536)
                        {
                            listener.Port = options.Port.Value;
                        }
                    });
                }

                if (options.Connector != "off")
                {
                    with.Connector.Enable();
                }

                if (options.Accept != null)
                {
                    with.Countries = options.Accept;
                }

                switch (options.Download)
                {
                    case "rarest-first":
                        with.Download.RarestFirst();
                        break;

                    case "sequential":
                        with.Download.Sequential();
                        break;
                }
            });

            if (options.Torrent != null)
            {
                client.Start(MetainfoFactory.FromFile(options.Torrent));
            }

            if (options.Hash != null)
            {
                client.Start(with =>
                {
                    with.Hash = FileHash.Parse(options.Hash);

                    if (options.Tracker != null)
                    {
                        foreach (string tracker in options.Tracker)
                        {
                            with.Trackers.Add(tracker);
                        }
                    }
                });
            }

            handle.WaitOne();
            Thread.Sleep(5000);
        }
    }
}