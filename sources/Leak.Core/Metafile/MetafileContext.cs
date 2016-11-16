using Leak.Core.Common;

namespace Leak.Core.Metafile
{
    public class MetafileContext
    {
        private readonly FileHash hash;
        private readonly string path;
        private readonly MetafileHooks hooks;
        private readonly MetafileDestination destination;

        private bool isCompleted;

        public MetafileContext(FileHash hash, string path, MetafileHooks hooks)
        {
            this.hash = hash;
            this.path = path;
            this.hooks = hooks;

            destination = new MetafileDestination(this);
        }

        public bool IsCompleted
        {
            get { return isCompleted; }
            set { isCompleted = value; }
        }

        public MetafileDestination Destination
        {
            get { return destination; }
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public string Path
        {
            get { return path; }
        }

        public MetafileHooks Hooks
        {
            get { return hooks; }
        }
    }
}