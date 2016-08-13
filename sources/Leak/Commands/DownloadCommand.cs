using Leak.Core.Client;
using Leak.Core.Common;
using Leak.Core.Metadata;
using Pargos;
using System;
using System.Threading;

namespace Leak.Commands
{
    public class DownloadCommand
    {
        private readonly ArgumentCollection arguments;

        public DownloadCommand(ArgumentCollection arguments)
        {
            this.arguments = arguments;
        }

        public void Execute()
        {
            string destination = arguments.GetString("destination");
            string torrent = arguments.GetString("torrent");

            string hash = arguments.GetString("hash");
            int trackers = arguments.Count("tracker");

            bool listen = arguments.GetString("listener") == "on";
            bool connect = arguments.GetString("connector") != "off";

            string portText = arguments.GetString("port");
            int? portValue = String.IsNullOrEmpty(portText) ? default(int?) : Int32.Parse(portText);

            ManualResetEvent handle = new ManualResetEvent(false);
            CompositeCallback callback = new CompositeCallback();
            LoggerFactory factory = new LoggerFactory(arguments);

            callback.Add(factory.Bouncer());
            callback.Add(factory.Connector());
            callback.Add(factory.Hash());
            callback.Add(factory.Listener());
            callback.Add(factory.Network());
            callback.Add(factory.Peer());

            callback.Add(new ReadyCallback(handle));

            PeerClient client = new PeerClient(with =>
            {
                with.Destination = destination;
                with.Callback = callback;

                if (listen)
                {
                    with.Listener.Enable(listener =>
                    {
                        if (portValue != null)
                        {
                            listener.Port = portValue.Value;
                        }
                    });
                }

                if (connect)
                {
                    with.Connector.Enable();
                }
            });

            if (torrent != null)
            {
                client.Start(MetainfoFactory.FromFile(torrent));
            }
            else if (hash != null)
            {
                client.Start(with =>
                {
                    with.Hash = FileHash.Parse(hash);

                    for (int i = 0; i < trackers; i++)
                    {
                        with.Trackers.Add(arguments.GetString("tracker", i));
                    }
                });
            }

            handle.WaitOne();
        }
    }
}