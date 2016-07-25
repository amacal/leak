using Leak.Core.Bencoding;
using Leak.Core.Common;
using Leak.Core.Messages;
using System;
using System.Collections.Generic;

namespace Leak.Core.Extensions
{
    public class Extender : ExtenderFormattable
    {
        private readonly object synchronized;
        private readonly ExtenderConfiguration configuration;
        private readonly Dictionary<PeerHash, ExtenderExtensionCollection> mapping;

        public Extender(Action<ExtenderConfiguration> configurer)
        {
            this.configuration = configurer.Configure(with =>
            {
                with.Callback = new ExtenderCallbackToNothing();
                with.Extensions = new ExtenderExtensionCollection();
                with.Handlers = new List<ExtenderHandler>();
            });

            this.synchronized = new object();
            this.mapping = new Dictionary<PeerHash, ExtenderExtensionCollection>();
        }

        public Extended GetHandshake()
        {
            List<BencodedEntry> extensions = new List<BencodedEntry>();

            foreach (ExtenderExtension item in configuration.Extensions)
            {
                extensions.Add(new BencodedEntry
                {
                    Key = new BencodedValue { Text = new BencodedText(item.Name) },
                    Value = new BencodedValue { Number = new BencodedNumber(item.Id) }
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

            return new Extended(0, Bencoder.Encode(payload));
        }

        public void Handle(PeerHash peer, ExtendedIncomingMessage message)
        {
            if (message.Id == 0)
            {
                BencodedValue value = Bencoder.Decode(message.ToBytes());
                ExtenderExtension[] extensions = FindExtensions(value);

                ExtenderExtensionCollection collection = new ExtenderExtensionCollection(extensions);
                ExtenderHandshake handshake = new ExtenderHandshake(collection);

                lock (synchronized)
                {
                    mapping.Add(peer, collection);
                }

                configuration.Callback.OnHandshake(peer, handshake);
            }
            else
            {
                string extension = configuration.Extensions.Translate(message.Id);
                ExtenderHandler handler = FindHandler(extension);

                handler?.Handle(peer, message);
            }
        }

        private static ExtenderExtension[] FindExtensions(BencodedValue value)
        {
            List<ExtenderExtension> extensions = new List<ExtenderExtension>();
            BencodedValue map = value.Find("m", x => x);

            if (map?.Dictionary != null)
            {
                foreach (BencodedEntry entry in map.Dictionary)
                {
                    byte? id = entry.Value?.Number?.ToByte();
                    string name = entry.Key?.Text?.GetString();

                    if (id != null && name != null)
                    {
                        extensions.Add(new ExtenderExtension(id.Value, name.ToLower()));
                    }
                }
            }

            return extensions.ToArray();
        }

        public byte Translate(PeerHash peer, string name)
        {
            lock (synchronized)
            {
                return mapping[peer].Translate(name);
            }
        }

        private ExtenderHandler FindHandler(string name)
        {
            foreach (ExtenderHandler handler in configuration.Handlers)
            {
                if (handler.CanHandle(name))
                {
                    return handler;
                }
            }

            return null;
        }
    }
}