using System;
using System.Collections.Generic;
using Leak.Core.Bencoding;

namespace Leak.Core.Net
{
    public class PeerExtendedMapping
    {
        private readonly PeerExtendedMappingConfiguration configuration;

        public PeerExtendedMapping()
        {
            configuration = new PeerExtendedMappingConfiguration();
        }

        public PeerExtendedMapping(Action<PeerExtendedMappingConfiguration> configurator)
        {
            configuration = new PeerExtendedMappingConfiguration();
            configurator.Invoke(configuration);
        }

        public byte? FindId(string name)
        {
            foreach (var extension in configuration.Extensions)
            {
                if (String.Equals(extension.Item1, name, StringComparison.OrdinalIgnoreCase))
                {
                    return Convert.ToByte(extension.Item2);
                }
            }

            return null;
        }

        public string FindName(int id)
        {
            foreach (var extension in configuration.Extensions)
            {
                if (extension.Item2 == id)
                {
                    return extension.Item1;
                }
            }

            return null;
        }

        public int? GetInt32(string name)
        {
            foreach (var extension in configuration.Properties)
            {
                if (extension.Item1 == name)
                {
                    return Convert.ToInt32(extension.Item2);
                }
            }

            return null;
        }

        public BencodedValue Encode()
        {
            List<BencodedEntry> extensions = new List<BencodedEntry>();
            List<BencodedEntry> properties = new List<BencodedEntry>();

            foreach (var extension in configuration.Extensions)
            {
                extensions.Add(new BencodedEntry
                {
                    Key = new BencodedValue { Text = new BencodedText(extension.Item1) },
                    Value = new BencodedValue { Number = new BencodedNumber(extension.Item2) }
                });
            }

            properties.Add(new BencodedEntry
            {
                Key = new BencodedValue { Text = new BencodedText("m") },
                Value = new BencodedValue { Dictionary = extensions.ToArray() }
            });

            return new BencodedValue
            {
                Dictionary = properties.ToArray()
            };
        }
    }
}