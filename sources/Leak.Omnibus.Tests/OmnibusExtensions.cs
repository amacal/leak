using Leak.Common;
using Leak.Events;

namespace Leak.Data.Map.Tests
{
    public static class OmnibusExtensions
    {
        public static void HandleMetafileVerified(this OmnibusService service, Metainfo metainfo)
        {
            service.Handle(new MetafileVerified
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