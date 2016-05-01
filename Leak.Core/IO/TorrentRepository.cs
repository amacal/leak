using Leak.Core.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Leak.Core.IO
{
    public class TorrentRepository
    {
        private readonly TorrentDirectory directory;
        private readonly TorrentBlockBitfield blocks;
        private readonly PeerBitfield pieces;
        private readonly List<TorrentTask> tasks;

        public TorrentRepository(TorrentDirectory directory)
        {
            this.directory = directory;
            this.blocks = new TorrentBlockBitfield(directory);
            this.pieces = new PeerBitfield(directory.Pieces.Count);
            this.tasks = new List<TorrentTask>();
        }

        public TorrentDirectory Directory
        {
            get { return directory; }
        }

        public TorrentPieceView Completed
        {
            get { return new TorrentPieceView(directory.Pieces, piece => pieces[piece.Index]); }
        }

        public void Initialize()
        {
            foreach (TorrentFile file in directory.Files)
            {
                string directory = Path.GetDirectoryName(file.Path);

                if (System.IO.Directory.Exists(directory) == false)
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                using (FileStream stream = File.Open(file.Path, FileMode.OpenOrCreate))
                {
                    if (stream.Length != file.Size)
                    {
                        stream.SetLength(file.Size);
                    }
                }
            }

            Parallel.ForEach(new[] { 0, 1, 2, 3 }, index =>
            {
                foreach (DataBlock target in AllBlocks(index, 4))
                {
                    using (HashAlgorithm algorithm = SHA1.Create())
                    {
                        bool equals = true;

                        byte[] source = target.Piece.Hash;
                        byte[] hash = algorithm.ComputeHash(target.Data, 0, target.Piece.Size);

                        for (int i = 0; i < 20; i++)
                        {
                            if (source[i] != hash[i])
                            {
                                equals = false;
                            }
                        }

                        if (equals)
                        {
                            pieces[target.Piece.Index] = true;

                            foreach (TorrentBlock block in target.Piece.Blocks)
                            {
                                blocks.Set(block);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Reschedule {target.Piece.Index}");
                        }
                    }
                }

                GC.Collect();
            });

            GC.Collect();
        }

        private IEnumerable<DataBlock> AllBlocks(int index, int count)
        {
            int offset = 0;
            byte[] data = new byte[directory.Pieces.Size];

            foreach (TorrentPiece piece in directory.Pieces)
            {
                offset = 0;

                foreach (TorrentFile file in directory.Files)
                {
                    if (file.Offset <= piece.Offset + piece.Size && piece.Offset < file.Offset + file.Size)
                    {
                        int availableInBuffer = data.Length - offset;
                        long acceptableByFile = file.Offset + file.Size - piece.Offset;
                        long fileOffset = piece.Offset - file.Offset;

                        if (acceptableByFile < availableInBuffer)
                        {
                            availableInBuffer = (int)acceptableByFile;
                        }

                        if (fileOffset < 0)
                        {
                            fileOffset = 0;
                        }

                        if (piece.Index % count == index)
                        {
                            lock (this)
                            {
                                using (FileStream stream = File.OpenRead(file.Path))
                                {
                                    stream.Seek(fileOffset, SeekOrigin.Begin);
                                    stream.Read(data, offset, availableInBuffer);
                                }
                            }
                        }

                        offset += availableInBuffer;
                    }
                }

                if (piece.Index % count == index)
                {
                    yield return new DataBlock
                    {
                        Piece = piece,
                        Data = data
                    };
                }
            }
        }

        public void Complete(TorrentBlock block, byte[] data)
        {
            blocks.Set(block);

            foreach (TorrentTask task in GetTasks(block, data))
            {
                using (FileStream stream = File.Open(task.File.Path, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    stream.Seek(task.FileOffset, SeekOrigin.Begin);
                    stream.Write(task.Data, task.DataOffset, task.Count);

                    stream.Flush();
                    stream.Close();
                }
            }
        }

        public void Complete(TorrentPiece piece)
        {
            pieces[piece.Index] = true;
        }

        public bool IsCompleted(TorrentBlock block)
        {
            return blocks.Has(block);
        }

        private IEnumerable<TorrentTask> GetTasks(TorrentBlock block, byte[] data)
        {
            int offset = 0;

            foreach (TorrentFile file in directory.Files)
            {
                if (file.Offset <= block.Offset + block.Size && block.Offset < file.Offset + file.Size)
                {
                    int available = data.Length - offset;
                    long acceptable = file.Offset + file.Size - block.Offset;
                    long fileOffset = block.Offset - file.Offset;

                    if (acceptable < available)
                    {
                        available = (int)acceptable;
                    }

                    if (fileOffset < 0)
                    {
                        fileOffset = 0;
                    }

                    yield return new TorrentTask
                    {
                        Block = block,
                        File = file,
                        FileOffset = fileOffset,
                        Data = data,
                        DataOffset = offset,
                        Count = available
                    };

                    offset += available;
                }
            }
        }

        private class TorrentTask
        {
            public TorrentBlock Block { get; set; }

            public TorrentFile File { get; set; }

            public long FileOffset { get; set; }

            public byte[] Data { get; set; }

            public int DataOffset { get; set; }

            public int Count { get; set; }
        }

        private class DataBlock
        {
            public TorrentPiece Piece { get; set; }

            public byte[] Data { get; set; }
        }
    }
}