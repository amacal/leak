using Leak.Common;
using Leak.Events;

namespace Leak.Omnibus.Tests
{
    public static class OmnibusExtensions
    {
        public static void HandleMetadataDiscovered(this OmnibusService service, Metainfo metainfo)
        {
            service.Handle(new MetadataDiscovered
            {
                Hash = service.Hash,
                Metainfo = metainfo
            });
        }

        public static void HandleDataVerified(this OmnibusService service, int pieces)
        {
            service.Handle(new DataVerified
            {
                Hash = service.Hash,
                Bitfield = new Bitfield(pieces)
            });
        }
    }
}
