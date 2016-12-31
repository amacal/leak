using System;
using Leak.Events;

namespace Leak.Metafile
{
    public class MetafileHooks
    {
        public Action<MetafileRead> OnMetafileRead;

        public Action<MetafileWritten> OnMetafileWritten;

        public Action<MetafileVerified> OnMetafileVerified;

        public Action<MetafileRejected> OnMetafileRejected;
    }
}