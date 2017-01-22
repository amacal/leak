using Leak.Bencoding;
using Leak.Common;

namespace Leak.Extensions.Metadata
{
    public class MetadataHandler : MoreHandler
    {
        private readonly MetadataHooks hooks;

        public MetadataHandler(MetadataHooks hooks)
        {
            this.hooks = hooks;
        }

        public void OnHandshake(FileHash hash, PeerHash peer, byte[] payload)
        {
            BencodedValue bencoded = Bencoder.Decode(payload);
            int? size = bencoded.Find("metadata_size", x => x?.ToInt32());

            if (size != null)
            {
                hooks.CallMetadataMeasured(hash, peer, size.Value);
            }
        }

        public void OnMessageReceived(FileHash hash, PeerHash peer, byte[] payload)
        {
            BencodedValue bencoded = Bencoder.Decode(payload);
            int? message = bencoded.Find("msg_type", x => x?.ToInt32());
            int? piece = bencoded.Find("piece", x => x?.ToInt32());
            int? size = bencoded.Find("total_size", x => x?.ToInt32());

            if (message == 0 && piece != null)
            {
                hooks.CallMetadataRequestReceived(hash, peer, piece.Value);
            }

            if (message == 1 && size != null)
            {
                hooks.CallMetadataMeasured(hash, peer, size.Value);
            }

            if (message == 1 && piece != null)
            {
                byte[] content = Bytes.Copy(payload, bencoded.Data.Length);
                hooks.MetadataPieceReceived(hash, peer, piece.Value, content);
            }

            if (message == 2 && piece != null)
            {
                hooks.CallMetadataRejectReceived(hash, peer, piece.Value);
            }
        }

        public void OnMessageSent(FileHash hash, PeerHash peer, byte[] payload)
        {
            BencodedValue bencoded = Bencoder.Decode(payload);
            int? message = bencoded.Find("msg_type", x => x?.ToInt32());
            int? piece = bencoded.Find("piece", x => x?.ToInt32());
            int? size = bencoded.Find("total_size", x => x?.ToInt32());

            if (message == 0 && piece != null)
            {
                hooks.CallMetadataRequestSent(hash, peer, piece.Value);
            }

            if (message == 1 && size != null)
            {
                hooks.CallMetadataMeasured(hash, peer, size.Value);
            }

            if (message == 1 && piece != null)
            {
                byte[] content = Bytes.Copy(payload, bencoded.Data.Length);
                hooks.MetadataPieceSent(hash, peer, piece.Value, content);
            }

            if (message == 2 && piece != null)
            {
                hooks.CallMetadataRejectSent(hash, peer, piece.Value);
            }
        }
    }
}