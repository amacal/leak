using Leak.Tasks;

namespace Leak.Data.Share
{
    public class DataShareTaskInterested : LeakTask<DataShareContext>
    {
        public void Execute(DataShareContext context)
        {
            if (context.Verified)
            {
                context.Dependencies.DataMap.Query((peer, bitfield, state) =>
                {
                    if (state.IsLocalChokingRemote && state.IsRemoteInterestedInLocal)
                    {
                        context.Dependencies.Glue.SendUnchoke(peer);
                    }
                });
            }
        }
    }
}