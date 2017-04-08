using System;
using System.Collections.Generic;
using System.IO;
using Leak.Common;
using Leak.Events;
using Leak.Networking.Core;

namespace Leak.Data.Store
{
    public static class RepositoryExtensions
    {
        public static void CallDataAllocated(this RepositoryHooks hooks, FileHash hash, string directory)
        {
            hooks.OnDataAllocated?.Invoke(new DataAllocated
            {
                Hash = hash,
                Directory = directory
            });
        }

        public static void CallDataVerified(this RepositoryHooks hooks, FileHash hash, Bitfield bitfield)
        {
            hooks.OnDataVerified?.Invoke(new DataVerified
            {
                Hash = hash,
                Bitfield = bitfield
            });
        }

        public static void CallBlockRead(this RepositoryHooks hooks, FileHash hash, BlockIndex block, DataBlock payload)
        {
            hooks.OnBlockRead?.Invoke(new BlockRead
            {
                Hash = hash,
                Block = block,
                Payload = payload
            });
        }

        public static void CallBlockWritten(this RepositoryHooks hooks, FileHash hash, BlockIndex block)
        {
            hooks.OnBlockWritten?.Invoke(new BlockWritten
            {
                Hash = hash,
                Block = block
            });
        }

        public static void CallPieceAccepted(this RepositoryHooks hooks, FileHash hash, PieceInfo piece)
        {
            hooks.OnPieceAccepted?.Invoke(new PieceAccepted
            {
                Hash = hash,
                Piece = piece
            });
        }

        public static void CallPieceRejected(this RepositoryHooks hooks, FileHash hash, PieceInfo piece)
        {
            hooks.OnPieceRejected?.Invoke(new PieceRejected
            {
                Hash = hash,
                Piece = piece
            });
        }

        public static string GetPath(this MetainfoEntry entry, string destination)
        {
            List<string> names = new List<string>();

            foreach (string name in entry.Name)
            {
                string value = name;

                foreach (char character in Path.GetInvalidFileNameChars())
                {
                    value = value.Replace(character, '-');
                }

                names.Add(value);
            }

            string path = String.Join(Path.DirectorySeparatorChar.ToString(), names);
            string result = Path.Combine(destination, path);

            return result;
        }
    }
}