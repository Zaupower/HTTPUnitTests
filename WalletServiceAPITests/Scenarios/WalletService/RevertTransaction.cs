using NUnit.Framework;
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
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests.Scenarios.WalletService
{
    public class RevertTransaction
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
            _observerWallet.OnCompleted();
            _observerUser.OnCompleted();
        }
        //If the transaction doesn’t exist =>   Code:  404; Message “ The given key was not present in the dictionary.”
        [Test]
        public async Task RevertTransaction_InvalidGuid_ReturnStatusIsNotFound
            ([Values("5eb58c84-2ea7-448d-8f93-69f65f93ccc3")] string invalidGuid)
        {
            string expectedstring = "The given key was not present in the dictionary.";
            var response = await _walletServiceProvider.RevertTransaction(invalidGuid);

            Assert.AreEqual(HttpStatusCode.NotFound, response.HttpStatusCode);
            Assert.AreEqual(expectedstring, response.Content);
            Assert.IsNull( response.Body);
        }
        //If the transaction is already reverted => Code: 500;
        //Message “Transaction '85d6766b-74b6-43cb-8372-b044a2994403' cannot be reversed due to 'Reverted' current status”
        [Test]
        public async Task RevertTransaction_InvalidGui_ReturnStatusIsNotFound()
        {
            //Precondition
            string expectedContent = "";                
            int userId = await CreateAndVerifyUser();
                //make charge
            ChargeModel charge = new ChargeModel
            {
                amount = 10,
                userId = userId,
            };
            var chargeResponse = await _walletServiceProvider.PostCharge(charge, true);
                //revert            
            await _walletServiceProvider.RevertTransaction(chargeResponse.Body);

            //Action
            var responseSecondRevert = await _walletServiceProvider.RevertTransaction(chargeResponse.Body);
            expectedContent = $"Transaction '{chargeResponse.Body}' cannot be reversed due to 'Reverted' current status";
            Assert.AreEqual(HttpStatusCode.InternalServerError, responseSecondRevert.HttpStatusCode);
            Assert.IsNull(responseSecondRevert.Body);
            Assert.AreEqual(expectedContent, responseSecondRevert.Content);
        }



        #region Helper

        public async Task<int> CreateAndVerifyUser(bool verifyUser = true)
        {
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
            if (verifyUser)
                await _userServiceProvider.SetUserStatus(userStatusModel);
            return responseCreateUser.Body;
        }
        #endregion

    }
}
