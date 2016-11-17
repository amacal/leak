using Leak.Core.Bencoding;
using Leak.Core.Common;

namespace Leak.Core.Glue.Extensions.Metadata
{
    public static class MetadataExtensions
    {
        public static void CallMetadataMeasured(this MetadataHooks hooks, FileHash hash, PeerHash peer, int size)
        {
            hooks.OnMetadataMeasured?.Invoke(new MetadataMeasured
            {
                Hash = hash,
                Peer = peer,
                Size = size
            });
        }

        public static void CallMetadataRequested(this MetadataHooks hooks, FileHash hash, PeerHash peer, int piece)
        {
            hooks.OnMetadataRequested?.Invoke(new MetadataRequested
            {
                Hash = hash,
                Peer = peer,
                Piece = piece
            });
        }

        public static void CallMetadataRejected(this MetadataHooks hooks, FileHash hash, PeerHash peer, int piece)
        {
            hooks.OnMetadataRejected?.Invoke(new MetadataRejected
            {
                Hash = hash,
                Peer = peer,
                Piece = piece
            });
        }

        public static void CallMetadataReceived(this MetadataHooks hooks, FileHash hash, PeerHash peer, int piece, byte[] data)
        {
            hooks.OnMetadataReceived?.Invoke(new MetadataReceived
            {
                Hash = hash,
                Peer = peer,
                Piece = piece,
                Data = data
            });
        }

        public static void SendMetadataRequest(this GlueService glue, PeerHash peer, int piece)
        {
            BencodedValue bencoded = new BencodedValue
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

            glue.SendExtension(peer, MetadataPlugin.Name, Bencoder.Encode(bencoded));
        }

        public static void SendMetadataReject(this GlueService glue, PeerHash peer, int piece)
        {
            BencodedValue bencoded = new BencodedValue
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
                        Value = new BencodedValue { Number = new BencodedNumber(piece) }
                    }
                }
            };

            glue.SendExtension(peer, MetadataPlugin.Name, Bencoder.Encode(bencoded));
        }

        public static void SendMetadataPiece(this GlueService glue, PeerHash peer, int piece, int total, byte[] data)
        {
            BencodedValue bencoded = new BencodedValue
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
                        Value = new BencodedValue { Number = new BencodedNumber(piece) }
                    },
                    new BencodedEntry
                    {
                        Key = new BencodedValue { Text = new BencodedText("total_size") },
                        Value = new BencodedValue { Number = new BencodedNumber(total) }
                    }
                }
            };

            byte[] payload = Bencoder.Encode(bencoded);

            Bytes.Append(ref payload, data);
            glue.SendExtension(peer, MetadataPlugin.Name, payload);
        }
    }
}