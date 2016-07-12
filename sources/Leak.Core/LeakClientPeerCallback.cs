using Leak.Core.Net;
using Leak.Core.Network;

namespace Leak.Core
{
    public class LeakClientPeerCallback : PeerCallback
    {
        private readonly LeakConfiguration configuration;
        private readonly LeakData data;

        public LeakClientPeerCallback(LeakConfiguration configuration, LeakData data)
        {
            this.configuration = configuration;
            this.data = data;
        }

        public override void OnAttached(PeerChannel channel)
        {
            data.Peers.Attach(channel);

            if (channel.Direction == NetworkConnectionDirection.Outgoing)
            {
                if (configuration.Extensions.IsEnabled)
                {
                    channel.Send(new PeerExtended(with =>
                    {
                        with.Handshake(configuration.Extensions.ToMapping());
                    }));
                }
            }
        }

        public override void OnTerminate(PeerChannel channel)
        {
        }

        public override void OnBitfield(PeerChannel channel, PeerBitfield message)
        {
        }

        public override void OnExtended(PeerChannel channel, PeerExtended extended)
        {
            if (configuration.Extensions.IsEnabled)
            {
                extended.Handle(channel, with =>
                {
                    with.Mapping = configuration.Extensions.ToMapping();
                    with.OnHandshake = OnHandshake;
                    with.OnMessage.Add("ut_metadata", OnMetadata);
                });
            }
        }

        private void OnHandshake(PeerChannel channel, PeerExtendedMapping mapping)
        {
            LeakCallbackExtensionsExchanged payload = new LeakCallbackExtensionsExchanged
            {
                SupportsMetadata = mapping.FindId("ut_metadata").HasValue
            };

            data.Peers.SetMapping(channel, mapping);
            configuration.Callback.OnExtensionsExchanged?.Invoke(payload);

            if (channel.Direction == NetworkConnectionDirection.Incoming)
            {
                channel.Send(new PeerExtended(with =>
                {
                    with.Handshake(configuration.Extensions.ToMapping());
                }));
            }

            foreach (LeakConfigurationSchedule schedule in configuration.Torrents.Schedules)
            {
                if (schedule.Operation == LeakConfigurationScheduleOperation.Metadata)
                {
                    channel.Send(new PeerExtended(with =>
                    {
                        with.Id = mapping.FindId("ut_metadata").Value;
                        with.MetadataRequest(request =>
                        {
                            request.Piece = 0;
                        });
                    }));
                }
            }
        }

        private void OnMetadata(PeerChannel channel, PeerExtended extended)
        {
            extended.HandleMetadata(channel, with =>
            {
                with.OnRequest = OnMetadataRequest;
                with.OnData = OnMetadataData;
            });
        }

        private void OnMetadataRequest(PeerChannel channel, PeerExtendedMetadataRequest request)
        {
            channel.Send(new PeerExtended(with =>
            {
                with.Id = data.Peers.GetMapping(channel).FindId("ut_metadata").Value;
                with.MetadataData(metadata =>
                {
                    metadata.Piece = request.Piece;
                    metadata.Data = data.Metainfo.GetData(channel.Hash, request.Piece);
                    metadata.TotalSize = data.Metainfo.GetTotalSize(channel.Hash);
                });
            }));
        }

        private void OnMetadataData(PeerChannel channel, PeerExtendedMetadataData request)
        {
            data.Metainfo.SetTotalSize(channel.Hash, request.TotalSize);
            data.Metainfo.SetData(channel.Hash, request.Piece, request.Data);

            if (data.Metainfo.IsCompleted(channel.Hash) == false)
            {
                channel.Send(new PeerExtended(with =>
                {
                    with.Id = data.Peers.GetMapping(channel).FindId("ut_metadata").Value;
                    with.MetadataRequest(metadata =>
                    {
                        metadata.Piece = request.Piece + 1;
                    });
                }));
            }
        }

        public override void OnHave(PeerChannel channel, PeerHave message)
        {
        }

        public override void OnInterested(PeerChannel channel, PeerInterested message)
        {
        }

        public override void OnKeepAlive(PeerChannel channel)
        {
        }

        public override void OnPiece(PeerChannel channel, PeerPiece message)
        {
        }

        public override void OnUnchoke(PeerChannel channel, PeerUnchoke message)
        {
        }
    }
}