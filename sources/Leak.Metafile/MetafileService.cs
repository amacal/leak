using System;
using Leak.Common;

namespace Leak.Meta.Store
{
    public interface MetafileService : IDisposable
    {
        FileHash Hash { get; }

        MetafileHooks Hooks { get; }

        MetafileParameters Parameters { get; }

        MetafileDependencies Dependencies { get; }

        void Start();

        void Read(int piece);

        void Write(int piece, byte[] data);

        void Verify();

        bool IsCompleted();
    }
}