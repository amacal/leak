using Leak.Bencoding;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Extensions
{
    public class MoreContainer : MoreMapping
    {
        private readonly Dictionary<byte, string> byId;
        private readonly Dictionary<string, byte> byCode;
        private readonly Dictionary<string, MoreHandler> handlers;

        public MoreContainer()
        {
            byId = new Dictionary<byte, string>();
            byCode = new Dictionary<string, byte>();
            handlers = new Dictionary<string, MoreHandler>();
        }

        public MoreContainer(BencodedValue bencoded)
        {
            byId = new Dictionary<byte, string>();
            byCode = new Dictionary<string, byte>();

            Decode(bencoded, byId, byCode);
        }

        public void Add(string code, MoreHandler handler)
        {
            byId.Add((byte)(byId.Count + 1), code);
            byCode.Add(code, (byte)(byCode.Count + 1));
            handlers.Add(code, handler);
        }

        public string[] ToArray()
        {
            return byCode.Keys.ToArray();
        }

        public bool Supports(string code)
        {
            return byCode.ContainsKey(code);
        }

        public byte Translate(string code)
        {
            return byCode[code];
        }

        public string Translate(byte id)
        {
            return byId[id];
        }

        public MoreHandler GetHandler(string code)
        {
            return handlers[code];
        }

        public IEnumerable<MoreHandler> AllHandlers()
        {
            return handlers.Values;
        }

        private static void Decode(BencodedValue bencoded, IDictionary<byte, string> byId, IDictionary<string, byte> byCode)
        {
            BencodedValue received = bencoded.Find("m", x => x);

            if (received?.Dictionary != null)
            {
                foreach (BencodedEntry entry in received.Dictionary)
                {
                    byte? id = entry.Value?.Number?.ToByte();
                    string code = entry.Key?.Text?.GetString();

                    if (id != null && code != null)
                    {
                        byId.Add(id.Value, code.ToLower());
                        byCode.Add(code.ToLower(), id.Value);
                    }
                }
            }
        }

        public BencodedValue Encode(int? size)
        {
            List<BencodedEntry> data = new List<BencodedEntry>();
            List<BencodedEntry> extensions = new List<BencodedEntry>();

            foreach (var item in byId)
            {
                extensions.Add(new BencodedEntry
                {
                    Key = new BencodedValue { Text = new BencodedText(item.Value) },
                    Value = new BencodedValue { Number = new BencodedNumber(item.Key) }
                });
            }

            data.Add(new BencodedEntry
            {
                Key = new BencodedValue { Text = new BencodedText("m") },
                Value = new BencodedValue { Dictionary = extensions.ToArray() }
            });

            if (size != null)
            {
                data.Add(new BencodedEntry
                {
                    Key = new BencodedValue { Text = new BencodedText("metadata_size") },
                    Value = new BencodedValue { Number = new BencodedNumber(size.Value) }
                });
            }

            return new BencodedValue
            {
                Dictionary = data.ToArray()
            };
        }
    }
}