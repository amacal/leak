using Leak.Core.Encoding;
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

        public byte[] Hash
        {
            get
            {
                return data.Find("info", GetHash);
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