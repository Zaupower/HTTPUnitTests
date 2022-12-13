using System;
using System.Collections.Concurrent;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests
{
    public class TestDataObserver : IObserver<string>
    {
        private static Lazy<TestDataObserver> _instance = new Lazy<TestDataObserver>(() => new TestDataObserver());
        public static TestDataObserver Instance => _instance.Value;

        private ConcurrentBag<string> _createdTransactionsIdCollection = new();
        public void OnCompleted()
        {
            _createdTransactionsIdCollection.Clear();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(string value)
        {
            _createdTransactionsIdCollection.Add(value);
        }

        public IEnumerable<string> GetAll()
        {
            //.Select(i=>i) to send copy over original reference
            return _createdTransactionsIdCollection.Select(i=>i);
        }
    }
}
