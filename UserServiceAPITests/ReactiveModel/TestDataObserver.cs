using System;
using System.Collections.Concurrent;

namespace UserServiceAPITests
{
    public class TestDataObserver : IObserver<int>
    {
        private static Lazy<TestDataObserver> _instance = new Lazy<TestDataObserver>(() => new TestDataObserver());
        public static TestDataObserver Instance => _instance.Value;

        private ConcurrentBag<int> _createdTransactionsIdCollection = new();
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(int value)
        {
            _createdTransactionsIdCollection.Add(value);
        }

        public IEnumerable<int> GetAll()
        {
            //.Select(i=>i) to send copy over original reference
            return _createdTransactionsIdCollection.Select(i=>i);
        }
    }
}
