using Leak.Core.Events;
using System;

namespace Leak.Core.Metafile
{
    public class MetafileHooks
    {
        public Action<MetafileWritten> OnMetafileWritten;

        public Action<MetafileVerified> OnMetafileVerified;

        public Action<MetafileRejected> OnMetafileRejected;
    }
}