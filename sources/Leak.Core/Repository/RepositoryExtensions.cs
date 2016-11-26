using Leak.Core.Common;
using Leak.Core.Events;
using Leak.Core.Metadata;
using System;
using System.Collections.Generic;
using System.IO;

namespace Leak.Core.Repository
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

        public static void CallDataWritten(this RepositoryHooks hooks, FileHash hash, RepositoryBlockData data)
        {
            hooks.OnDataWritten?.Invoke(new DataWritten
            {
                Hash = hash,
                Piece = data.Piece,
                Block = data.Offset % 16384,
                Size = data.Length
            });
        }

        public static void CallDataAccepted(this RepositoryHooks hooks, FileHash hash, int piece)
        {
            hooks.OnDataAccepted?.Invoke(new DataAccepted
            {
                Hash = hash,
                Piece = piece
            });
        }

        public static void CallDataRejected(this RepositoryHooks hooks, FileHash hash, int piece)
        {
            hooks.OnDataRejected?.Invoke(new DataRejected
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