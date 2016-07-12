namespace Leak.Core
{
    public class LeakConfiguration
    {
        public LeakConfiguration()
        {
            Callback = new LeakConfigurationCallback();
            Listener = new LeakConfigurationListener();
            Torrents = new LeakConfigurationTorrentCollection();
            Extensions = new LeakConfigurationExtensionCollection();
        }

        public LeakConfigurationCallback Callback { get; set; }

        public LeakConfigurationListener Listener { get; set; }

        public LeakConfigurationTorrentCollection Torrents { get; set; }

        public LeakConfigurationExtensionCollection Extensions { get; set; }
    }
}