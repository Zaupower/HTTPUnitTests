using System;
using System.Collections.Concurrent;

namespace WalletServiceAPITests
{
    public class TestDataObserver : IObserver<string>
    {
        private ConcurrentBag<string> _createdTransactionsIdCollection = new();
        public void OnCompleted()
        {
            throw new NotImplementedException();
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
