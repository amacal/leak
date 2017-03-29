using Leak.Tasks;

namespace Leak.Data.Get
{
    public class DataGetTaskInterested : LeakTask<DataGetContext>
    {
        public void Execute(DataGetContext context)
        {
            if (context.Verified)
            {
                context.Dependencies.DataMap.Query((peer, bitfield, state) =>
                {
                    if (state.IsLocalInterestedInRemote == false && bitfield?.Completed > 0)
                    {
                        context.Dependencies.Glue.SendInterested(peer);
                    }
                });
            }
        }
    }
}