using Leak.Core.Common;
using Leak.Core.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Leak.Core.Repository
{
    public class RepositoryTaskVerifyRange : RepositoryTask
    {
        private readonly Bitfield scope;

        public RepositoryTaskVerifyRange(Bitfield scope)
        {
            this.scope = scope;
        }

        public void Accept(RepositoryTaskVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Execute(RepositoryContext context)
        {
            int length = context.Metainfo.Pieces.Length;
            int size = context.Metainfo.Properties.PieceSize;

            FileHash hash = context.Metainfo.Hash;
            Bitfield bitfield = new Bitfield(length);

            int count = Environment.ProcessorCount;
            TaskData data = new TaskData
            {
                Context = context,
                Bitfield = bitfield,
                Reading = new SemaphoreSlim(0, count),
                Writing = new SemaphoreSlim(count, count),
                Pieces = new Queue<PieceData>(),
                Buffer = new RotatingBuffer(count, size),
                Left = scope.Completed
            };

            List<Task> tasks = new List<Task>
            {
                BufferPieces(data)
            };

            for (int i = 0; i < count; i++)
            {
                tasks.Add(VerifyPieces(data));
            }

            Task.WaitAll(tasks.ToArray());
            data.Context.Callback.OnVerified(hash, bitfield);

            data.Reading.Dispose();
            data.Writing.Dispose();
        }

        private Task BufferPieces(TaskData data)
        {
            return Task.Run(() =>
            {
                int piece = 0;
                bool seek = false;

                Metainfo metainfo = data.Context.Metainfo;
                string destination = data.Context.Destination;

                using (RepositoryStream stream = new RepositoryStream(destination, metainfo))
                {
                    RotatingEntry buffer;
                    int read = metainfo.Properties.PieceSize;

                    stream.Seek(0, SeekOrigin.Begin);

                    while (piece < metainfo.Properties.Pieces)
                    {
                        if (scope[piece] == false)
                        {
                            seek = true;
                            piece++;
                        }
                        else
                        {
                            if (seek)
                            {
                                stream.Seek(piece * metainfo.Properties.PieceSize, SeekOrigin.Begin);
                            }

                            data.Writing.Wait();
                            buffer = data.Buffer.Next();

                            read = stream.Read(buffer.Bytes, 0, read);
                            piece = piece + 1;

                            lock (data.Pieces)
                            {
                                data.Pieces.Enqueue(new PieceData
                                {
                                    Index = piece - 1,
                                    Hash = metainfo.Pieces[piece - 1],
                                    Data = buffer,
                                    Size = read
                                });
                            }

                            data.Reading.Release(1);
                        }
                    }
                }
            });
        }

        private Task VerifyPieces(TaskData data)
        {
            return Task.Run(() =>
            {
                byte[] hash;
                PieceData piece = null;
                TimeSpan delay = TimeSpan.FromSeconds(2);

                using (HashAlgorithm algorithm = SHA1.Create())
                {
                    do
                    {
                        if (data.Reading.Wait(delay))
                        {
                            lock (data.Pieces)
                            {
                                piece = data.Pieces.Dequeue();
                            }

                            hash = algorithm.ComputeHash(piece.Data.Bytes, 0, piece.Size);

                            if (Bytes.Equals(hash, piece.Hash.ToBytes()))
                            {
                                lock (data.Bitfield)
                                {
                                    data.Bitfield[piece.Index] = true;
                                }
                            }

                            data.Buffer.Release(piece.Data);
                            data.Writing.Release(1);

                            lock (data)
                            {
                                data.Left--;
                            }
                        }
                    } while (data.Left > 0);
                }
            });
        }

        private class TaskData
        {
            public SemaphoreSlim Reading { get; set; }

            public SemaphoreSlim Writing { get; set; }

            public RepositoryContext Context { get; set; }

            public Queue<PieceData> Pieces { get; set; }

            public RotatingBuffer Buffer { get; set; }

            public Bitfield Bitfield { get; set; }

            public int Left { get; set; }
        }

        private class PieceData
        {
            public int Index { get; set; }

            public MetainfoHash Hash { get; set; }

            public RotatingEntry Data { get; set; }

            public int Size { get; set; }
        }

        private class RotatingBuffer
        {
            public readonly RotatingEntry[] entries;

            public RotatingBuffer(int count, int size)
            {
                entries = new RotatingEntry[count];

                for (int i = 0; i < count; i++)
                {
                    entries[i] = new RotatingEntry
                    {
                        Available = true,
                        Bytes = new byte[size]
                    };
                }
            }

            public RotatingEntry Next()
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    if (entries[i].Available)
                    {
                        entries[i].Available = false;
                        return entries[i];
                    }
                }

                return null;
            }

            public void Release(RotatingEntry entry)
            {
                entry.Available = true;
            }
        }

        private class RotatingEntry
        {
            public bool Available { get; set; }

            public byte[] Bytes { get; set; }
        }
    }
}