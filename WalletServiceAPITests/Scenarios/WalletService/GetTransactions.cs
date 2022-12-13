using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Models.Responses.Base;
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
            var response = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> responseTransactions = response.Body;
            
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.IsEmpty(responseTransactions);
            Assert.AreEqual(response.Content, "[]");
        }

        //If the user isn’t verified => Returns a list of all user transactions
        [Test]
        public async Task GetTransactions_UnverifiedUser_ReturnStatusIsOkAndEmptyArray()
        {
            //Create user => get id
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "firstName_test_getTransaction_unverifiedUser",
                lastName = "lastName_test_getTransaction_unverifiedUser"
            };
            //Create User
            HttpResponse<int> responseCreateUser = await _userServiceProvider.CreateUser(request);
            SetUserStatusModel userStatusModel = new SetUserStatusModel
            {
                UserId = responseCreateUser.Body,
                NewStatus = true
            };
            //Set user status true
            await _userServiceProvider.SetUserStatus(userStatusModel);
            ChargeModel chargeModel = new ChargeModel
            {
                amount = 10,
                userId = responseCreateUser.Body,
            };
            //Make transaction
            await _walletServiceProvider.PostCharge(chargeModel);
            //Set status false
            userStatusModel.NewStatus = false;
            await _userServiceProvider.SetUserStatus(userStatusModel);
            
            //Action
            var response = await _walletServiceProvider.GetTransactions(responseCreateUser.Body);
            List<GetTransactionModel> responseTransactions = response.Body;
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.IsNotEmpty(responseTransactions);
        }

        //If there are no transactions by user => empty array
        [Test]
        public async Task GetTransactions_ValidUserNoTransactions_ReturnStatusIsOkAndEmptyArray
            ([Values(false, true) ] bool activeUser)
        {
            //Precondition
            //Create User
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "firstName_test_getTransaction_unverifiedUser",
                lastName = "lastName_test_getTransaction_unverifiedUser"
            };
            //Create User
            HttpResponse<int> responseCreateUser = await _userServiceProvider.CreateUser(request);
            //Verify User
            //Set user status true
            
            SetUserStatusModel userStatusModel = new SetUserStatusModel
            {
                UserId = responseCreateUser.Body,
                NewStatus = true
            };
            if(activeUser)
                await _userServiceProvider.SetUserStatus(userStatusModel);

            ChargeModel chargeModel = new ChargeModel
            {
                amount = 10,
                userId = responseCreateUser.Body,
            };
            //Action
            var response = await _walletServiceProvider.GetTransactions(responseCreateUser.Body);
            List<GetTransactionModel> responseTransactions = response.Body;
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.IsEmpty(responseTransactions);
        }




    }
}
