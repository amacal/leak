using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using System.IO;

namespace Leak.Core.Repository
{
    public class ResourceRepositoryToHash : ResourceRepository
    {
        private readonly FileHash hash;
        private readonly string location;

        public ResourceRepositoryToHash(FileHash hash, string location)
        {
            this.hash = hash;
            this.location = location;
        }

        public MetainfoProperties Properties
        {
            get { throw new System.NotImplementedException(); }
        }

        public Bitfield Initialize()
        {
            throw new System.NotImplementedException();
        }

        public ResourceRepository WithMetainfo(out Metainfo metainfo)
        {
            string path = Path.Combine(location, $"{hash}.metainfo");
            byte[] data = File.ReadAllBytes(path);

            metainfo = MetainfoFactory.FromBytes(data);
            string destination = Path.Combine(location, hash.ToString());

            return new ResourceRepositoryToMetainfo(metainfo, destination);
        }

        public ResourceRepositorySession OpenSession()
        {
            return new ResourceRepositoryToHashSession(hash, location);
        }

        public static Metainfo Open(string location, FileHash hash)
        {
            string path = Path.Combine(location, $"{hash}.metainfo");

            if (File.Exists(path) == false)
                return null;

            return MetainfoFactory.FromBytes(File.ReadAllBytes(path));
        }
    }
}