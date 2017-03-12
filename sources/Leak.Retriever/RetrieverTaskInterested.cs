using Leak.Tasks;

namespace Leak.Dataget
{
    public class RetrieverTaskInterested : LeakTask<RetrieverContext>
    {
        public void Execute(RetrieverContext context)
        {
            context.Dependencies.Omnibus.Query((peer, bitfield, state) =>
            {
                if (state.IsLocalInterestedInRemote == false && bitfield?.Completed > 0)
                {
                    context.Dependencies.Glue.SendInterested(peer);
                }
            });
        }
    }
}