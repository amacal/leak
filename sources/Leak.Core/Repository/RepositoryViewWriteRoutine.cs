using Leak.Files;

namespace Leak.Core.Repository
{
    public class RepositoryViewWriteRoutine
    {
        private readonly RepositoryViewWriteCallback callback;
        private readonly RepositoryViewEntry[] entries;
        private readonly FileBuffer buffer;
        private readonly long offset;
        private readonly int piece;

        public RepositoryViewWriteRoutine(int piece, RepositoryViewEntry[] entries, FileBuffer buffer, long offset, RepositoryViewWriteCallback callback)
        {
            this.piece = piece;
            this.entries = entries;
            this.buffer = buffer;
            this.offset = offset;
            this.callback = callback;
        }

        public void Execute()
        {
            long position = offset - entries[0].Start;
            Receiver receiver = new Receiver(this);

            entries[0].File.Write(position, buffer, receiver.OnCompleted);
        }

        private class Receiver
        {
            private readonly RepositoryViewWriteRoutine routine;
            private readonly int index;
            private readonly int completed;

            public Receiver(RepositoryViewWriteRoutine routine)
            {
                this.routine = routine;
            }

            private Receiver(RepositoryViewWriteRoutine routine, int index, int completed)
            {
                this.routine = routine;
                this.index = index;
                this.completed = completed;
            }

            public void OnCompleted(FileWrite data)
            {
                if (data.Count > 0)
                {
                    long offset = data.Position + data.Count;
                    long position = completed + data.Count + routine.offset - routine.entries[index].Start;

                    Receiver receiver = new Receiver(routine, index, completed + data.Count);
                    File file = routine.entries[index].File;

                    if (routine.entries.Length > index + 1 && position >= routine.entries[index].Start)
                    {
                        receiver = new Receiver(routine, index + 1, completed + data.Count);
                        file = routine.entries[index + 1].File;
                        offset = 0;
                    }

                    if (completed + data.Count == routine.buffer.Count)
                    {
                        routine.callback.Invoke(new RepositoryViewWrite
                        {
                            Piece = routine.piece,
                            Buffer = routine.buffer,
                            Count = routine.buffer.Count
                        });
                    }
                    else
                    {
                        file.Write(offset, new FileBuffer(routine.buffer.Data, routine.buffer.Offset + data.Count, routine.buffer.Offset - data.Count), receiver.OnCompleted);
                    }
                }
                else
                {
                    routine.callback.Invoke(new RepositoryViewWrite
                    {
                        Piece = routine.piece,
                        Buffer = routine.buffer,
                        Count = completed
                    });
                }
            }
        }
    }
}