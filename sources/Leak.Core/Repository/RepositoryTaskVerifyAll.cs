using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Leak.Core.Repository
{
    public class RepositoryTaskVerifyAll : LeakTask<RepositoryContext>
    {
        public void Execute(RepositoryContext context)
        {
            FileHash hash = context.Metainfo.Hash;
            Bitfield bitfield = new Bitfield(context.Metainfo.Pieces.Length);

            int count = Environment.ProcessorCount;
            TaskData data = new TaskData
            {
                Context = context,
                Bitfield = bitfield,
                Reading = new SemaphoreSlim(0, count),
                Writing = new SemaphoreSlim(count, count),
                Pieces = new Queue<PieceData>(),
                Left = context.Metainfo.Pieces.Length
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

                Metainfo metainfo = data.Context.Metainfo;
                string destination = data.Context.Destination;

                using (RepositoryStream stream = new RepositoryStream(destination, metainfo))
                {
                    byte[] buffer;
                    int read = metainfo.Properties.PieceSize;

                    stream.Seek(0, SeekOrigin.Begin);

                    while (piece < metainfo.Properties.Pieces)
                    {
                        data.Writing.Wait();
                        buffer = new byte[metainfo.Properties.PieceSize];

                        read = stream.Read(buffer, 0, read);
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

                            hash = algorithm.ComputeHash(piece.Data, 0, piece.Size);

                            if (Bytes.Equals(hash, piece.Hash.ToBytes()))
                            {
                                lock (data.Bitfield)
                                {
                                    data.Bitfield[piece.Index] = true;
                                }
                            }

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

            public Bitfield Bitfield { get; set; }

            public int Left { get; set; }
        }

        private class PieceData
        {
            public int Index { get; set; }

            public MetainfoHash Hash { get; set; }

            public byte[] Data { get; set; }

            public int Size { get; set; }
        }
    }
}