using Leak.Core.Events;
using System;

namespace Leak.Core.Metafile
{
    public class MetafileHooks
    {
        public Action<MetadataDiscovered> OnMetadataDiscovered;
    }
}