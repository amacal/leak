using Leak.Core.Bencoding;
using Leak.Core.Common;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Cando.Metadata
{
    public class MetadataHandler : CandoHandler
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

        public void Handle(PeerHash peer, Extended payload)
        {
            BencodedValue value = Bencoder.Decode(payload.Data);
            int? type = value.Find("msg_type", x => x?.ToInt32());
            int? piece = value.Find("piece", x => x?.ToInt32());
            int? size = value.Find("total_size", x => x.ToInt32());

            if (type != null && piece != null)
            {
                if (type == 1 && size != null)
                {
                    byte[] content = Bytes.Copy(payload.Data, value.Data.Length);
                    MetadataData data = new MetadataData(piece.Value, size.Value, content);

                    configuration.Callback.OnData(peer, data);
                }
            }
        }
    }
}