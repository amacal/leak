using Leak.Tasks;

namespace Leak.Data.Share
{
    public class DatashareTaskInterested : LeakTask<DatashareContext>
    {
        public void Execute(DatashareContext context)
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