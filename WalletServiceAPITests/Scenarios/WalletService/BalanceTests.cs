﻿using NUnit.Framework;
using System.Net;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests.Scenarios.WalletService
{
    
    public class BalanceTests
    {
        private WalletServiceServiceProvider _serviceProvider = WalletServiceServiceProvider.Instance;
        private TestDataObserver _observer;

        [OneTimeSetUp]
        public void setup()
        {
            _observer = new TestDataObserver();
            _serviceProvider.Subscribe(_observer);
        }
        [OneTimeTearDown]
        public async Task teardown()
        {
            foreach (var transactionMade in _observer.GetAll())
            {
                await _serviceProvider.RevertTransaction(transactionMade);
            }
        }
        [Test]
        public async Task GetBalance_VerifiedUser_ResponseStatusIsOk([Values(3)] int userId)
        {
            //Precondition

            //Action
            var response = await _serviceProvider.GetBalance(userId);            
            
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);            
            Assert.IsTrue(response.Body is double);       
        }

        [Test]
        public async Task GetBalance_UnverifiedAndInvalidUser_ResponseStatusIsOk([Values(0,1)] int userId)
        {
            //Precondition
            double expectedBody = 0;
            //Action
            var response = await _serviceProvider.GetBalance(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(expectedBody, response.Body);
            Assert.AreEqual(response.Content, "not active user");
        }
    }
}
