using NUnit.Framework;
using System.Net;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests.Scenarios.WalletService
{
    
    public class BalanceTests
    {
        private WalletServiceServiceProvider _walletServiceProvider = WalletServiceServiceProvider.Instance;
        private WalletServiceAPITests.TestDataObserver _observerCharge;
        private WalletServiceAPITests.TestDataObserverDeleteAction _observerRevert;

        [OneTimeSetUp]
        public void setup()
        {
            _observerCharge = TestDataObserver.Instance;
            _observerRevert = TestDataObserverDeleteAction.Instance;

            _walletServiceProvider.Subscribe(_observerCharge);
            _walletServiceProvider.SubscribeRevert(_observerRevert);
        }
        [OneTimeTearDown]
        public async Task teardown()
        {
            //Clean Transactions
            List<string> newCharges = _observerCharge.GetAll().ToList();
            List<string> revertedCharges = _observerRevert.GetAll().ToList();

            List<string> resultList = newCharges.Except(revertedCharges).ToList();

            foreach (var userCreated in resultList)
            {
                await _walletServiceProvider.RevertTransaction(userCreated);
            }

            _observerCharge.OnCompleted();
            _observerRevert.OnCompleted();
        }
        [Test]
        public async Task GetBalance_VerifiedUser_ResponseStatusIsOk([Values(3)] int userId)
        {
            //Precondition

            //Action
            var response = await _walletServiceProvider.GetBalance(userId);            
            
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
            var response = await _walletServiceProvider.GetBalance(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(expectedBody, response.Body);
            Assert.AreEqual(response.Content, "not active user");
        }
    }
}
