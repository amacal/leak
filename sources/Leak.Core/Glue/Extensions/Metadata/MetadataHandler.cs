using Leak.Core.Bencoding;
using Leak.Core.Common;

namespace Leak.Core.Glue.Extensions.Metadata
{
    public class MetadataHandler : GlueHandler
    {
        private readonly MetadataHooks hooks;

        public MetadataHandler(MetadataHooks hooks)
        {
            this.hooks = hooks;
        }

        public void Handle(FileHash hash, PeerHash peer, byte[] payload)
        {
            BencodedValue bencoded = Bencoder.Decode(payload);
            int? message = bencoded.Find("msg_type", x => x?.ToInt32());
            int? piece = bencoded.Find("piece", x => x?.ToInt32());

            if (message == 0)
            {
                hooks.CallMetadataRequestReceived(hash, peer, piece.Value);
            }
        }
    }
}