using Leak.Core.Common;
using Leak.Core.Events;
using Leak.Core.Messages;
using Leak.Core.Omnibus;
using Leak.Core.Repository;
using System;

namespace Leak.Core.Retriever.Components
{
    public static class RetrieverExtensions
    {
        public static void Write(this RepositoryService repository, int piece, int offset, DataBlock data)
        {
            repository.Write(new RepositoryBlockData(piece, offset, data));
        }

        public static OmnibusStrategy ToOmnibus(this RetrieverStrategy strategy)
        {
            switch (strategy)
            {
                case RetrieverStrategy.RarestFirst:
                    return OmnibusStrategy.RarestFirst;

                case RetrieverStrategy.Sequential:
                    return OmnibusStrategy.Sequential;
            }

            throw new NotSupportedException();
        }

        public static void CallDataChanged(this RetrieverHooks hooks, FileHash hash, int completed)
        {
            hooks.OnDataChanged?.Invoke(new DataChanged
            {
                Hash = hash,
                Completed = completed
            });
        }

        public static void CallDataCompleted(this RetrieverHooks hooks, FileHash hash)
        {
            hooks.OnDataCompleted?.Invoke(new DataCompleted
            {
                Hash = hash
            });
        }
    }
}