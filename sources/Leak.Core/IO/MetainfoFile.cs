using Leak.Core.Encoding;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Leak.Core.IO
{
    public class MetainfoFile
    {
        private readonly BencodedValue data;

        public MetainfoFile(byte[] data)
        {
            this.data = Bencoder.Decode(data);
        }

        public MetainfoFile(Action<MetainfoFileConfiguration> callback)
        {
            MetainfoFileConfiguration configuration = new MetainfoFileConfiguration
            {
                Includes = new List<string>(),
                Trackers = new List<string>(),
                PieceLength = 16384
            };

            callback.Invoke(configuration);
            data = Bencoder.Decode(configuration.ToBinary());
        }

        public byte[] Hash
        {
            get
            {
                return data.Find("info", GetHash);
            }
        }

        public byte[] Data
        {
            get
            {
                return data.Find("info", x => x.Data.GetBytes());
            }
        }

        public MetainfoTrackerCollection Trackers
        {
            get
            {
                return new MetainfoTrackerCollection(data);
            }
        }

        public MetainfoEntryCollection Entries
        {
            get
            {
                return data.Find("info", x => new MetainfoEntryCollection(x));
            }
        }

        public MetainfoPieceCollection Pieces
        {
            get
            {
                return data.Find("info", x => new MetainfoPieceCollection(x));
            }
        }

        private byte[] GetHash(BencodedValue value)
        {
            using (HashAlgorithm algorithm = SHA1.Create())
            {
                return algorithm.ComputeHash(value.Data.GetBytes());
            }
        }
    }
}