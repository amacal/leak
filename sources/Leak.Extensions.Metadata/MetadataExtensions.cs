using Leak.Bencoding;
using Leak.Common;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Metadata
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

        public static void CallMetadataRequestSent(this MetadataHooks hooks, FileHash hash, PeerHash peer, int piece)
        {
            hooks.OnMetadataRequestSent?.Invoke(new MetadataRequested
            {
                Hash = hash,
                Peer = peer,
                Piece = piece
            });
        }

        public static void CallMetadataRequestReceived(this MetadataHooks hooks, FileHash hash, PeerHash peer, int piece)
        {
            hooks.OnMetadataRequestReceived?.Invoke(new MetadataRequested
            {
                Hash = hash,
                Peer = peer,
                Piece = piece
            });
        }

        public static void CallMetadataRejectSent(this MetadataHooks hooks, FileHash hash, PeerHash peer, int piece)
        {
            hooks.OnMetadataRejectSent?.Invoke(new MetadataRejected
            {
                Hash = hash,
                Peer = peer,
                Piece = piece
            });
        }

        public static void CallMetadataRejectReceived(this MetadataHooks hooks, FileHash hash, PeerHash peer, int piece)
        {
            hooks.OnMetadataRejectReceived?.Invoke(new MetadataRejected
            {
                Hash = hash,
                Peer = peer,
                Piece = piece
            });
        }

        public static void MetadataPieceSent(this MetadataHooks hooks, FileHash hash, PeerHash peer, int piece, byte[] data)
        {
            hooks.OnMetadataPieceSent?.Invoke(new MetadataReceived
            {
                Hash = hash,
                Peer = peer,
                Piece = piece,
                Data = data
            });
        }

        public static void MetadataPieceReceived(this MetadataHooks hooks, FileHash hash, PeerHash peer, int piece, byte[] data)
        {
            hooks.OnMetadataPieceReceived?.Invoke(new MetadataReceived
            {
                Hash = hash,
                Peer = peer,
                Piece = piece,
                Data = data
            });
        }

        public static void SendMetadataRequest(this CoordinatorService glue, PeerHash peer, int piece)
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

        public static void SendMetadataReject(this CoordinatorService glue, PeerHash peer, int piece)
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

        public static void SendMetadataPiece(this CoordinatorService glue, PeerHash peer, int piece, int total, byte[] data)
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