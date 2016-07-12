using System;

namespace Leak.Core
{
    public class LeakConfigurationCallback
    {
        internal Action<LeakCallbackHandshakeExchanged> OnHandshakeExchanged { get; set; }

        internal Action<LeakCallbackExtensionsExchanged> OnExtensionsExchanged { get; set; }

        internal Action<LeakCallbackMetadataDownloaded> OnMetadataDownloaded { get; set; }
    }
}