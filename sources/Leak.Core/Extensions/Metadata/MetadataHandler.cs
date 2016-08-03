using Leak.Core.Bencoding;
using Leak.Core.Common;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Extensions.Metadata
{
    public class MetadataHandler : ExtenderHandler
    {
        private readonly MetadataConfiguration configuration;

        public MetadataHandler(Action<MetadataConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Callback = new MetadataCallbackNothing();
            });
        }

        public bool CanHandle(string name)
        {
            return name == "ut_metadata";
        }

        public void Handle(PeerHash peer, ExtendedIncomingMessage message)
        {
            BencodedValue value = Bencoder.Decode(message.ToBytes());
            int? type = value.Find("msg_type", x => x?.ToInt32());
            int? piece = value.Find("piece", x => x?.ToInt32());
            int? size = value.Find("total_size", x => x.ToInt32());

            if (type != null && piece != null)
            {
                if (type == 1 && size != null)
                {
                    byte[] payload = message.ToBytes(value.Data.Length);
                    MetadataData data = new MetadataData(piece.Value, size.Value, payload);

                    configuration.Callback.OnData(peer, data);
                }
            }
        }
    }
}