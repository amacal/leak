using Leak.Core.Bencoding;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Glue
{
    public class GlueMore
    {
        private readonly Dictionary<byte, string> byId;
        private readonly Dictionary<string, byte> byCode;

        public GlueMore()
        {
            byId = new Dictionary<byte, string>();
            byCode = new Dictionary<string, byte>();
        }

        public GlueMore(BencodedValue bencoded)
        {
            byId = new Dictionary<byte, string>();
            byCode = new Dictionary<string, byte>();

            Decode(bencoded, byId, byCode);
        }

        public void Add(string name, GlueHandler handler)
        {
            byId.Add((byte)(byId.Count + 1), name);
            byCode.Add(name, (byte)(byCode.Count + 1));
        }

        public string[] ToArray()
        {
            return byCode.Keys.ToArray();
        }

        private static void Decode(BencodedValue bencoded, IDictionary<byte, string> byId, IDictionary<string, byte> byName)
        {
            BencodedValue received = bencoded.Find("m", x => x);

            if (received?.Dictionary != null)
            {
                foreach (BencodedEntry entry in received.Dictionary)
                {
                    byte? id = entry.Value?.Number?.ToByte();
                    string name = entry.Key?.Text?.GetString();

                    if (id != null && name != null)
                    {
                        byId.Add(id.Value, name.ToLower());
                        byName.Add(name.ToLower(), id.Value);
                    }
                }
            }
        }

        public BencodedValue Encode()
        {
            List<BencodedEntry> extensions = new List<BencodedEntry>();

            foreach (var item in byId)
            {
                extensions.Add(new BencodedEntry
                {
                    Key = new BencodedValue { Text = new BencodedText(item.Value) },
                    Value = new BencodedValue { Number = new BencodedNumber(item.Key) }
                });
            }

            BencodedValue payload = new BencodedValue
            {
                Dictionary = new[]
                {
                    new BencodedEntry
                    {
                        Key = new BencodedValue { Text = new BencodedText("m") },
                        Value = new BencodedValue { Dictionary = extensions.ToArray() }
                    }
                }
            };

            return payload;
        }
    }
}