using Leak.Core.Bencoding;
using Leak.Core.Messages;

namespace Leak.Core.Cando.Metadata
{
    public static class MetadataExtensions
    {
        public static Extended MetadataRequest(this CandoFormatter formattable, int piece)
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

            return new Extended(formattable.Translate("ut_metadata"), Bencoder.Encode(payload));
        }
    }
}