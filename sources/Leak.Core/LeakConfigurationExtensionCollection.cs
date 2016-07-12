using Leak.Core.Net;

namespace Leak.Core
{
    public class LeakConfigurationExtensionCollection
    {
        internal bool IsEnabled { get; set; }

        internal bool SupportsMetadata { get; set; }

        public PeerHandshakeOptions ToOptions()
        {
            PeerHandshakeOptions options = PeerHandshakeOptions.None;

            if (IsEnabled)
            {
                options |= PeerHandshakeOptions.Extended;
            }

            return options;
        }

        public PeerExtendedMapping ToMapping()
        {
            return new PeerExtendedMapping(with =>
            {
                if (SupportsMetadata)
                {
                    with.Extension("ut_metadata", 3);
                }
            });
        }
    }
}