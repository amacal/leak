using Leak.Core.Bencoding;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Cando
{
    public class CandoMap
    {
        private readonly Dictionary<byte, string> byId;
        private readonly Dictionary<string, byte> byName;

        public CandoMap()
        {
            byId = new Dictionary<byte, string>();
            byName = new Dictionary<string, byte>();
        }

        public void Add(byte value, string name)
        {
            if (value > 0)
            {
                byId.Add(value, name);
                byName.Add(name, value);
            }
        }

        public string[] All()
        {
            return byName.Keys.ToArray();
        }

        public string Translate(byte id)
        {
            string name;
            byId.TryGetValue(id, out name);
            return name;
        }

        public byte Translate(string name)
        {
            byte id;
            byName.TryGetValue(name, out id);
            return id;
        }

        public static CandoMap Parse(BencodedValue value)
        {
            CandoMap map = new CandoMap();
            BencodedValue received = value.Find("m", x => x);

            if (received?.Dictionary != null)
            {
                foreach (BencodedEntry entry in received.Dictionary)
                {
                    byte? id = entry.Value?.Number?.ToByte();
                    string name = entry.Key?.Text?.GetString();

                    if (id != null && name != null)
                    {
                        map.Add(id.Value, name.ToLower());
                    }
                }
            }

            return map;
        }

        public BencodedValue ToBencoded()
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