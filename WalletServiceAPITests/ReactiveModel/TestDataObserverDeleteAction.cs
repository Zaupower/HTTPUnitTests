using System;
using System.Collections.Concurrent;

namespace WalletServiceAPITests
{
    public class TestDataObserverDeleteAction : IObserver<string>
    {
        private static Lazy<TestDataObserverDeleteAction> _instance = new Lazy<TestDataObserverDeleteAction>(() => new TestDataObserverDeleteAction());
        public static TestDataObserverDeleteAction Instance => _instance.Value;

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
            return _createdTransactionsIdCollection.Select(i => i);
        }
    }
}