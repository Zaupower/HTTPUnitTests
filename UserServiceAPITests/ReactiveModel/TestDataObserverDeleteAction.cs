using System;
using System.Collections.Concurrent;

namespace UserServiceAPITests
{
    public class TestDataObserverDeleteAction : IObserver<int>
    {
        private static Lazy<TestDataObserverDeleteAction> _instance = new Lazy<TestDataObserverDeleteAction>(() => new TestDataObserverDeleteAction());
        public static TestDataObserverDeleteAction Instance => _instance.Value;

        public int Key { get; internal set; }
        private ConcurrentBag<int> _createdTransactionsIdCollection = new();
        public void OnCompleted()
        {
            _createdTransactionsIdCollection.Clear();
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
