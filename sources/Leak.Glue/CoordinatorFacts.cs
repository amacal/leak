using System.Collections.Generic;
using Leak.Bencoding;
using Leak.Common;
using Leak.Events;
using Leak.Extensions;
using Leak.Peer.Coordinator.Core;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorFacts
    {
        private readonly MoreContainer more;
        private MetafileVerified metadata;
        private Bitfield mine;

        public CoordinatorFacts()
        {
            more = new MoreContainer();
        }

        public Bitfield Bitfield
        {
            get { return mine; }
        }

        public void Install(IReadOnlyCollection<MorePlugin> plugins)
        {
            foreach (MorePlugin plugin in plugins)
            {
                plugin.Install(more);
            }
        }

        public void Handle(MetafileVerified data)
        {
            metadata = data;
        }

        public void Handle(DataVerified data)
        {
            mine = data.Bitfield;
        }

        public Bitfield ApplyHave(Bitfield other, int piece)
        {
            if (other == null && metadata?.Metainfo != null)
            {
                other = new Bitfield(metadata.Metainfo.Pieces.Length);
            }

            if (other != null && other.Length > piece)
            {
                other[piece] = true;
            }

            return other;
        }

        public Bitfield ApplyBitfield(Bitfield other, Bitfield received)
        {
            if (metadata?.Metainfo != null)
            {
                received = new Bitfield(metadata.Metainfo.Pieces.Length, received);
            }

            return received;
        }

        public Extended GetHandshake()
        {
            BencodedValue encoded = more.Encode(metadata?.Size);
            byte[] binary = Bencoder.Encode(encoded);

            return new Extended(0, binary);
        }

        public string[] GetExtensions()
        {
            return more.ToArray();
        }

        public string Translate(byte id, out MoreHandler handler)
        {
            string code = more.Translate(id);
            handler = more.GetHandler(code);

            return code;
        }

        public MoreHandler GetHandler(string code)
        {
            return more.GetHandler(code);
        }

        public IEnumerable<MoreHandler> AllHandlers()
        {
            return more.AllHandlers();
        }
    }
}