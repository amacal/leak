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

        public MetainfoFileTrackerCollection Trackers
        {
            get
            {
                return new MetainfoFileTrackerCollection(data);
            }
        }

        public MetainfoFileEntryCollection Entries
        {
            get
            {
                return data.Find("info", x => new MetainfoFileEntryCollection(x));
            }
        }

        private byte[] GetHash(BencodedValue value)
        {
            using (HashAlgorithm algorithm = SHA1.Create())
            {
                return algorithm.ComputeHash(value.ToHex());
            }
        }
    }
}