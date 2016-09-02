using Leak.Core.Metadata;
using System;
using System.Collections.Generic;
using System.IO;

namespace Leak.Core.Repository
{
    public static class RepositoryExtensions
    {
        public static RepositoryBlock ToBlock(this RepositoryBlockData data)
        {
            return new RepositoryBlock(data.Piece, data.Offset, data.Length);
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