namespace Leak
{
    public class EventProcessor
    {
        private readonly EventProcessorConfiguration configuration;

        public EventProcessor(EventProcessorConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Process(string type, dynamic data)
        {
            configuration.GetHandler(type).Invoke(data);
        }
    }
}