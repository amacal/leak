using Leak.Core.Bencoding;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Extensions.Metadata
{
    public static class MetadataExtensions
    {
        public static bool MetadataSupports(this ExtenderFormattable formattable, PeerHash peer)
        {
            return formattable.Supports(peer, "ut_metadata");
        }

        public static Extended MetadataRequest(this ExtenderFormattable formattable, PeerHash peer, int piece)
        {
            BencodedValue payload = new BencodedValue
            {
                Dictionary = new[]
                {
                    new BencodedEntry
                    {
                        Key = new BencodedValue {Text = new BencodedText("msg_type")},
                        Value = new BencodedValue {Number = new BencodedNumber(0)}
                    },
                    new BencodedEntry
                    {
                        Key = new BencodedValue {Text = new BencodedText("piece")},
                        Value = new BencodedValue {Number = new BencodedNumber(piece)}
                    }
                }
            };

            return new Extended(formattable.Translate(peer, "ut_metadata"), Bencoder.Encode(payload));
        }
    }
}