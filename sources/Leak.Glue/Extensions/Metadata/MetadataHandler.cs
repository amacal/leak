using Leak.Bencoding;
using Leak.Common;

namespace Leak.Glue.Extensions.Metadata
{
    public class MetadataHandler : GlueHandler
    {
        private readonly MetadataHooks hooks;

        public MetadataHandler(MetadataHooks hooks)
        {
            this.hooks = hooks;
        }

        public void HandleMessage(FileHash hash, PeerHash peer, byte[] payload)
        {
            BencodedValue bencoded = Bencoder.Decode(payload);
            int? message = bencoded.Find("msg_type", x => x?.ToInt32());
            int? piece = bencoded.Find("piece", x => x?.ToInt32());
            int? size = bencoded.Find("total_size", x => x?.ToInt32());

            if (message == 0 && piece != null)
            {
                hooks.CallMetadataRequested(hash, peer, piece.Value);
            }

            if (message == 1 && size != null)
            {
                hooks.CallMetadataMeasured(hash, peer, size.Value);
            }

            if (message == 1 && piece != null)
            {
                byte[] content = Bytes.Copy(payload, bencoded.Data.Length);
                hooks.CallMetadataReceived(hash, peer, piece.Value, content);
            }

            if (message == 2 && piece != null)
            {
                hooks.CallMetadataRejected(hash, peer, piece.Value);
            }
        }
    }
}