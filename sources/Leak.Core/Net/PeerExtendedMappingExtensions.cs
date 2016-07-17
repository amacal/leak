using System;
using Leak.Core.Bencoding;

namespace Leak.Core.Net
{
    public static class PeerExtendedMappingExtensions
    {
        public static void Extension(this PeerExtendedMappingConfiguration configuration, string name, int id)
        {
            configuration.Extensions.Add(Tuple.Create(name, id));
        }

        public static void Property(this PeerExtendedMappingConfiguration configuration, string name, int value)
        {
            configuration.Properties.Add(Tuple.Create(name, (object)value));
        }

        public static void Property(this PeerExtendedMappingConfiguration configuration, string name, string value)
        {
            configuration.Properties.Add(Tuple.Create(name, (object)value));
        }

        public static void Decode(this PeerExtendedMappingConfiguration configuration, BencodedValue value)
        {
            foreach (BencodedEntry entry in value.Dictionary)
            {
                string key = entry.Key.Text.GetString();

                if (key == "m")
                {
                    DecodeExtensions(configuration, entry.Value);
                }
                else if (entry.Value.Text != null)
                {
                    configuration.Property(key, entry.Value.Text.GetString());
                }
                else if (entry.Value.Number != null)
                {
                    configuration.Property(key, entry.Value.Number.ToInt32());
                }
            }
        }

        private static void DecodeExtensions(PeerExtendedMappingConfiguration configuration, BencodedValue value)
        {
            foreach (BencodedEntry entry in value.Dictionary)
            {
                string name = entry.Key.Text.GetString();
                int id = entry.Value.Number.ToInt32();

                configuration.Extension(name, id);
            }
        }
    }
}