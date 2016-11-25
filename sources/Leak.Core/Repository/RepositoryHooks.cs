using Leak.Core.Events;
using System;

namespace Leak.Core.Repository
{
    public class RepositoryHooks
    {
        public Action<DataAllocated> OnDataAllocated;

        public Action<DataVerified> OnDataVerified;

        public Action<DataWritten> OnDataWritten;

        public Action<DataAccepted> OnDataAccepted;

        public Action<DataRejected> OnDataRejected;
    }
}