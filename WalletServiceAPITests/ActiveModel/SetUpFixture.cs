﻿using NUnit.Framework;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests
{
    //CLASS NOT WORKING PROPERLY FOR EVERY TEST, MAYBE NEED TO MAKE ALL TESTS IN THE SAME NAMESPACE
    [SetUpFixture]
    public class SetUpFixture
    {
        //private WalletServiceServiceProvider _provider = WalletServiceServiceProvider.Instance;
        //private TestDataObserver _observer;

        //[OneTimeSetUp]
        //public async Task OneTimeSetUp()
        //{
        //    _observer = new TestDataObserver();
        //    _provider.Subscribe(_observer);
        //}

        //[OneTimeTearDown]
        //public async Task OneTimeTearDown()
        //{            
        //    //Active Model
        //    //await provider.ReverseAllTransactions();

        //    //Reactive Model
        //    foreach(var transactionsMade in _observer.GetAll())
        //    {
        //        await _provider.RevertTransaction(transactionsMade);
        //    }
        //}
    }
}
