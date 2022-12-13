using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.ServiceProvider;
using WalletServiceAPITests.Models.Requests.WalletService;
using WalletServiceAPITests.Models.Responses.WalletService;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests.Scenarios.WalletService
{
    public class GetTransactions
    {
        private WalletServiceServiceProvider _walletServiceProvider = WalletServiceServiceProvider.Instance;
        private UserServiceServiceProvider _userServiceProvider = UserServiceServiceProvider.Instance;

        private WalletServiceAPITests.TestDataObserver _observerWallet;
        private UserServiceAPITests.TestDataObserver _observerUser;

        [OneTimeSetUp]
        public void setup()
        {
            _observerWallet = new WalletServiceAPITests.TestDataObserver();
            _observerUser = new UserServiceAPITests.TestDataObserver();

            _walletServiceProvider.Subscribe(_observerWallet);
            _userServiceProvider.Subscribe(_observerUser);
        }
        [OneTimeTearDown]
        public async Task teardown()
        {
            foreach (var transactionMade in _observerWallet.GetAll())
            {
                await _walletServiceProvider.RevertTransaction(transactionMade);
            }

            foreach (var userCreated in _observerUser.GetAll())
            {
                await _userServiceProvider.DeleteUser(userCreated);
            }

        }

        //If user doesn’t exist => empty array
        [Test]
        public async Task GetTransactions_InvalidUserId_ReturnStatusIsOkAndEmptyArray
            ([Values(0, -1)] int userId)
        {
            var response = await _serviceProvider.GetTransactions(userId);
            List<GetTransactionModel> responseTransactions = response.Body;
            
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.IsEmpty(responseTransactions);
            Assert.AreEqual(response.Content, "[]");
        }

        //If the user isn’t verified => Returns a list of all user transactions
        [Test]
        public async Task GetTransactions_UnverifiedUser_ReturnStatusIsOkAndEmptyArray
            ([Values(3,5)] int userId)
        {
            //Create user => get id
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "firstName_test_getTransaction_unverifiedUser",
                lastName = "lastName_test_getTransaction_unverifiedUser"
            };

            //Action
            HttpResponse<int> responseCreateUser = await _serviceProvider.CreateUser(request);

            var response = await _serviceProvider.GetTransactions(userId);
            List<GetTransactionModel> responseTransactions = response.Body;

            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.IsNotEmpty(responseTransactions);
        }

        //If there are no transactions by user => empty array
        [Test]
        public async Task GetTransactions_ValidUserNoTransactions_ReturnStatusIsOkAndEmptyArray
            ([Values(3, 5)] int userId)
        {
            var response = await _serviceProvider.GetTransactions(userId);
            List<GetTransactionModel> responseTransactions = response.Body;

            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.IsNotEmpty(responseTransactions);
        }




    }
}
