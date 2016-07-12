using Leak.Core.Encoding;
using System;

namespace Leak.Core.Net
{
    public static class PeerExtendedMetadataExtensions
    {
        public static void MetadataRequest(this PeerExtendedConfiguration configuration, Action<PeerExtendedMetadataRequest> configurer)
        {
            PeerExtendedMetadataRequest metadata = new PeerExtendedMetadataRequest();
            configurer.Invoke(metadata);

            BencodedValue request = new BencodedValue
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
                        Value = new BencodedValue { Number = new BencodedNumber(metadata.Piece) }
                    }
                }
            };

            configuration.Bencoded(request);
        }

        public static void MetadataReject(this PeerExtendedConfiguration configuration, Action<PeerExtendedMetadataReject> configurer)
        {
            PeerExtendedMetadataReject metadata = new PeerExtendedMetadataReject();
            configurer.Invoke(metadata);

            BencodedValue request = new BencodedValue
            {
                Dictionary = new[]
                {
                    new BencodedEntry
                    {
                        Key = new BencodedValue { Text = new BencodedText("msg_type") },
                        Value = new BencodedValue { Number = new BencodedNumber(2) }
                    },
                    new BencodedEntry
                    {
                        Key = new BencodedValue { Text = new BencodedText("piece") },
                        Value = new BencodedValue { Number = new BencodedNumber(metadata.Piece) }
                    }
                }
            };

            configuration.Bencoded(request);
        }

        public static void MetadataData(this PeerExtendedConfiguration configuration, Action<PeerExtendedMetadataData> configurer)
        {
            PeerExtendedMetadataData metadata = new PeerExtendedMetadataData();
            configurer.Invoke(metadata);

            BencodedValue request = new BencodedValue
            {
                Dictionary = new[]
                {
                    new BencodedEntry
                    {
                        Key = new BencodedValue { Text = new BencodedText("msg_type") },
                        Value = new BencodedValue { Number = new BencodedNumber(1) }
                    },
                    new BencodedEntry
                    {
                        Key = new BencodedValue { Text = new BencodedText("piece") },
                        Value = new BencodedValue { Number = new BencodedNumber(metadata.Piece) }
                    },
                    new BencodedEntry
                    {
                        Key = new BencodedValue { Text = new BencodedText("total_size") },
                        Value = new BencodedValue { Number = new BencodedNumber(metadata.TotalSize) }
                    }
                }
            };

            configuration.Bencoded(request, metadata.Data);
        }

        public static void HandleMetadata(this PeerExtended message, PeerChannel channel, Action<PeerExtendedMetadataCallback> configurer)
        {
            PeerExtendedMetadataCallback callback = new PeerExtendedMetadataCallback();
            configurer.Invoke(callback);

            BencodedValue decoded = message.Decode();
            int type = decoded.Find("msg_type", value => value.Number.ToInt32());

            if (type == 0)
            {
                callback.OnRequest.Invoke(channel, new PeerExtendedMetadataRequest
                {
                    Piece = decoded.Find("piece", value => value.Number.ToInt32())
                });
            }

            if (type == 1)
            {
                int size = decoded.Data.Length;
                byte[] data = new byte[message.Length - size];

                Array.Copy(message.Content, size, data, 0, data.Length);

                callback.OnData?.Invoke(channel, new PeerExtendedMetadataData
                {
                    Piece = decoded.Find("piece", value => value.Number.ToInt32()),
                    TotalSize = decoded.Find("total_size", value => value.Number.ToInt32()),
                    Data = data
                });
            }

            if (type == 2)
            {
                callback.OnReject?.Invoke(channel, new PeerExtendedMetadataReject
                {
                    Piece = decoded.Find("piece", value => value.Number.ToInt32())
                });
            }
        }
    }
}