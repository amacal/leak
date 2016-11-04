using NUnit.Framework;

namespace Leak.Completion.Tests
{
    public class CompletionThreadTests
    {
        [Test]
        public void CanStart()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                worker.Start();
            }
        }
    }
}