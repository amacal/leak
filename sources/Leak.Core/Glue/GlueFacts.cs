using Leak.Core.Common;
using Leak.Core.Metadata;

namespace Leak.Core.Glue
{
    public class GlueFacts
    {
        private Metainfo metainfo;
        private readonly GlueMore more;

        public GlueFacts(GlueConfiguration configuration)
        {
            more = new GlueMore();
            metainfo = configuration.Metainfo;
        }

        public void Install(GluePlugin[] plugins)
        {
            foreach (GluePlugin plugin in plugins)
            {
                plugin.Install(more);
            }
        }

        public void ApplyMetainfo(Metainfo value)
        {
            metainfo = value;
        }

        public Bitfield ApplyHave(Bitfield bitfield, int piece)
        {
            if (bitfield == null && metainfo != null)
            {
                bitfield = new Bitfield(metainfo.Pieces.Length);
            }

            if (bitfield != null && bitfield.Length > piece)
            {
                bitfield[piece] = true;
            }

            return bitfield;
        }

        public Bitfield ApplyBitfield(Bitfield bitfield, Bitfield received)
        {
            if (metainfo != null)
            {
                received = new Bitfield(metainfo.Pieces.Length, received);
            }

            return received;
        }
    }
}