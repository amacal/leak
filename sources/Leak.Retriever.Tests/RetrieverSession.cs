using System;
using Moq;

namespace Leak.Retriever.Tests
{
    public class RetrieverSession : IDisposable
    {
        private readonly RetrieverService retriever;
        private readonly RetrieverData data;
        private readonly Mock<RetrieverRepository> repository;

        public RetrieverSession(RetrieverService retriever, RetrieverData data, Mock<RetrieverRepository> repository)
        {
            this.retriever = retriever;
            this.data = data;
            this.repository = repository;
        }

        public RetrieverService Retriever
        {
            get { return retriever; }
        }

        public RetrieverData Data
        {
            get { return data; }
        }

        public Mock<RetrieverRepository> Repository
        {
            get { return repository; }
        }

        public void Dispose()
        {
        }
    }
}
