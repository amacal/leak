using System;
using System.Threading;

namespace Leak.Completion
{
    public class CompletionThread : CompletionWorker, IDisposable
    {
        private Thread thread;
        private bool completed;
        private IntPtr port;

        public void Start()
        {
            port = CompletionInterop.CreateIoCompletionPort(new IntPtr(-1), IntPtr.Zero, 0, 0);
            thread = new Thread(Execute);

            thread.Start();
        }

        private unsafe void Execute()
        {
            CompletionInterop.OverlappedEntry[] entries
                = new CompletionInterop.OverlappedEntry[1024];

            while (completed == false)
            {
                uint processed;

                bool result = CompletionInterop.GetQueuedCompletionStatusEx(
                    port,
                    entries,
                    entries.Length,
                    out processed,
                    1000,
                    0);

                if (result)
                {
                    for (int i = 0; i < processed; i++)
                    {
                        CompletionInterop.OverlappedEntry entry = entries[i];
                        Overlapped overlapped = Overlapped.Unpack(entry.lpOverlapped);
                        CompletionCallback callback = overlapped.AsyncResult as CompletionCallback;

                        if (result)
                        {
                            callback?.Complete(entry.lpOverlapped, (int)entry.dwNumberOfBytesTransferred);
                        }
                        else
                        {
                            callback?.Fail(entry.lpOverlapped);
                        }

                        Overlapped.Free(entry.lpOverlapped);
                    }
                }
            }
        }

        public void Add(IntPtr handle)
        {
            CompletionInterop.CreateIoCompletionPort(handle, port, (uint)handle.ToInt32(), 0);
        }

        public void Dispose()
        {
            completed = true;
            thread?.Join();
            thread = null;
        }
    }
}