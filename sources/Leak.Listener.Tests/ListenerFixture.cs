using System;
using System.Threading.Tasks;
using Leak.Completion;
using Leak.Events;
using Leak.Networking;
using Leak.Tasks;

namespace Leak.Listener.Tests
{
    public class ListenerFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;
        private PeerListener listener;

        private readonly PeerListenerHooks hooks;

        public ListenerFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            worker = new CompletionThread();
            worker.Start();

            pool = new NetworkPoolFactory(pipeline, worker).CreateInstance(new NetworkPoolHooks());
            pool.Start();

            hooks = new PeerListenerHooks();
        }

        public PeerListenerHooks Hooks
        {
            get { return hooks; }
        }

        public Task<ListenerSession> Start()
        {
            Action<ListenerStarted> onStarted = hooks.OnListenerStarted;
            TaskCompletionSource<ListenerSession> completion = new TaskCompletionSource<ListenerSession>();

            PeerListenerConfiguration configuration = new PeerListenerConfiguration
            {
                Port = new PeerListenerPortRandom()
            };

            hooks.OnListenerStarted = data =>
            {
                onStarted?.Invoke(data);
                completion.SetResult(new ListenerSession(data.Port, pool));
            };

            listener = new PeerListener(pool, hooks, configuration);
            listener.Start();

            return completion.Task;
        }

        public void Dispose()
        {
            listener?.Stop();
            listener?.Dispose();

            worker?.Dispose();
            pipeline?.Stop();
        }
    }
}
