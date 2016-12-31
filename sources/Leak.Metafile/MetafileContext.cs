using Leak.Common;

namespace Leak.Metafile
{
    public class MetafileContext
    {
        private readonly MetafileParameters parameters;
        private readonly MetafileHooks hooks;
        private readonly MetafileDestination destination;

        private bool isCompleted;

        public MetafileContext(MetafileParameters parameters, MetafileHooks hooks)
        {
            this.parameters = parameters;
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

        public MetafileParameters Parameters
        {
            get { return parameters; }
        }

        public MetafileHooks Hooks
        {
            get { return hooks; }
        }
    }
}