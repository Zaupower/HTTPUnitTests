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
        //If user doesn’t exist => empty array
        [Test]
        public async Task GetTransactions_InvalidUserId_ReturnStatusIsOkAndEmptyArray
            ([Values(0, -1)] int userId)
        {
            //Action
            var response = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> responseTransactions = response.Body;
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.IsEmpty(responseTransactions);
            Assert.AreEqual(response.Content, "[]");
        }

        //If the user isn’t verified => Returns a list of all user transactions
        [Test]
        public async Task GetTransactions_UnverifiedUser_ReturnStatusIsOkAndEmptyArray()
        {

            //Precondition
            int userId = await CreateAndVerifyUser();

            ChargeModel chargeModel = new ChargeModel
            {
                amount = 10,
                userId = userId,
            };
                //Make transaction
            await _walletServiceProvider.PostCharge(chargeModel);
                //Set status false
            SetUserStatusModel userStatusModel = new SetUserStatusModel
            {
                UserId = userId,
                NewStatus = false
            };
            await _userServiceProvider.SetUserStatus(userStatusModel);
            
            //Action
            var response = await _walletServiceProvider.GetTransactions(userId);
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
            int userId = await CreateAndVerifyUser(activeUser);

            //Action
            var response = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> responseTransactions = response.Body;
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.IsEmpty(responseTransactions);
        }
        [Test]
        public async Task GetTransactions_TransactionStatus_CorrectStatus
            ([Values( 2, 1)] int expedctedTrtansactionStatus)
        {
           
            //Precondition
            int userId = await CreateAndVerifyUser();
            bool isReverted = expedctedTrtansactionStatus == 2? true:false;
            ChargeModel chargeModel = new ChargeModel
            {
                amount = 10,
                userId = userId,
            };
                //Make transaction
            var chargeResponse = await _walletServiceProvider.PostCharge(chargeModel, isReverted);
            string transactionId = chargeResponse.Body;

            if (expedctedTrtansactionStatus ==2)            
                await _walletServiceProvider.RevertTransaction(transactionId);

            //Action
            var getTransactionsResponse = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> transactions = getTransactionsResponse.Body;
            GetTransactionModel transaction = transactions.FirstOrDefault(i => i.transactionId == transactionId);
            
            //Assert
            Assert.AreEqual(chargeModel.amount, transaction.amount);
            Assert.AreEqual(chargeModel.userId, transaction.userId);
            Assert.AreEqual(expedctedTrtansactionStatus, transaction.status);

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
