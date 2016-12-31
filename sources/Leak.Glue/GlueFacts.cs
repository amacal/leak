using System.Collections.Generic;
using Leak.Bencoding;
using Leak.Common;
using Leak.Communicator.Messages;
using Leak.Events;
using Leak.Extensions;

namespace Leak.Glue
{
    public class GlueFacts
    {
        private readonly MoreContainer more;
        private MetafileVerified metadata;

        public GlueFacts()
        {
            more = new MoreContainer();
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

        public Bitfield ApplyHave(Bitfield bitfield, int piece)
        {
            if (bitfield == null && metadata?.Metainfo != null)
            {
                bitfield = new Bitfield(metadata.Metainfo.Pieces.Length);
            }

            if (bitfield != null && bitfield.Length > piece)
            {
                bitfield[piece] = true;
            }

            return bitfield;
        }

        public Bitfield ApplyBitfield(Bitfield bitfield, Bitfield received)
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