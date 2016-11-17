using Leak.Core.Bencoding;
using Leak.Core.Common;

namespace Leak.Core.Glue.Extensions.Metadata
{
    public static class MetadataExtensions
    {
        public static void CallMetadataRequestReceived(this MetadataHooks hooks, FileHash hash, PeerHash peer, int piece)
        {
            hooks.OnMetadataRequestReceived?.Invoke(new MetadataRequest
            {
                Hash = hash,
                Peer = peer,
                Piece = piece
            });
        }

        public static void SendMetadataRequest(this GlueService glue, PeerHash peer, int piece)
        {
            BencodedValue payload = new BencodedValue
            {
                Dictionary = new[]
                {
                    new BencodedEntry
                    {
                        Key = new BencodedValue { Text = new BencodedText("msg_type") },
                        Value = new BencodedValue { Number = new BencodedNumber(0) }
                    },
                    new BencodedEntry
                    {
                        Key = new BencodedValue { Text = new BencodedText("piece") },
                        Value = new BencodedValue { Number = new BencodedNumber(piece) }
                    }
                }
            };

            glue.SendExtension(peer, MetadataPlugin.Name, Bencoder.Encode(payload));
        }
    }
}