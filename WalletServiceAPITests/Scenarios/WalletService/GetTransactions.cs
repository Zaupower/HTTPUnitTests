using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletServiceAPITests.Models.Requests.WalletService;
using WalletServiceAPITests.Models.Responses.WalletService;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests.Scenarios.WalletService
{
    public class GetTransactions
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

        //If user doesn’t exist => empty array
        [Test]
        public async Task GetTransactions_InvalidUserId_ReturnStatusIsOkAndEmptyArray
            ([Values(0, -1)] int userId, [Values(0,10,-20, 20.008)] double amount)
        {
            var response = await _serviceProvider.GetTransactions(userId);
            List<GetTransactionModel> responseTransactions = response.Body;

            Assert.IsEmpty(responseTransactions);

        }



        
    }
}
