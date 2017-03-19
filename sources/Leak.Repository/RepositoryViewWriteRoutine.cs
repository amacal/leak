using Leak.Files;

namespace Leak.Data.Store
{
    public class RepositoryViewWriteRoutine
    {
        private readonly RepositoryViewWriteCallback callback;
        private readonly RepositoryViewEntry[] entries;
        private readonly FileBuffer buffer;
        private readonly long offset;
        private readonly int piece;
        private readonly int block;

        public RepositoryViewWriteRoutine(RepositoryViewCache cache, int piece, int block, FileBuffer buffer, RepositoryViewWriteCallback callback)
        {
            this.piece = piece;
            this.block = block;
            this.buffer = buffer;
            this.callback = callback;

            this.entries = cache.Find(piece, block, 1);
            this.offset = piece * (long)cache.PieceSize + block * cache.BlockSize;
        }

        public void Execute()
        {
            long position = offset - entries[0].Start;
            long count = entries[0].Size - position;

            if (count >= buffer.Count)
            {
                count = buffer.Count;
            }

            Receiver receiver = new Receiver(this);
            FileBuffer data = new FileBuffer(buffer.Data, buffer.Offset, (int)count);

            entries[0].File.Write(position, data, receiver.OnCompleted);
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
                    RepositoryViewEntry entry = routine.entries[index];

                    if (routine.entries.Length > index + 1 && position >= routine.entries[index].Size)
                    {
                        receiver = new Receiver(routine, index + 1, completed + data.Count);
                        entry = routine.entries[index + 1];
                        position = 0;
                        offset = 0;
                    }

                    if (completed + data.Count == routine.buffer.Count)
                    {
                        routine.callback.Invoke(new RepositoryViewWrite
                        {
                            Piece = routine.piece,
                            Block = routine.block,
                            Buffer = routine.buffer,
                            Count = routine.buffer.Count
                        });
                    }
                    else
                    {
                        int left = routine.buffer.Count - completed - data.Count;
                        long count = entry.Size - position;

                        if (count > left)
                        {
                            count = left;
                        }

                        entry.File.Write(offset, new FileBuffer(routine.buffer.Data, routine.buffer.Offset + completed + data.Count, (int)count), receiver.OnCompleted);
                    }
                }
                else
                {
                    routine.callback.Invoke(new RepositoryViewWrite
                    {
                        Piece = routine.piece,
                        Block = routine.block,
                        Buffer = routine.buffer,
                        Count = completed
                    });
                }
            }
        }
    }
}