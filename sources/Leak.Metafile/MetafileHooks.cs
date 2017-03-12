using System;
using Leak.Events;

namespace Leak.Meta.Store
{
    public class MetafileHooks
    {
        public Action<MetafileRead> OnMetafileRead;

        public Action<MetafileWritten> OnMetafileWritten;

        public Action<MetafileVerified> OnMetafileVerified;

        public Action<MetafileRejected> OnMetafileRejected;
    }
}