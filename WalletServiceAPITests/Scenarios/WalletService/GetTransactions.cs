using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Extensions;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.ServiceProvider;
using WalletServiceAPITests.Models.Requests.WalletService;
using WalletServiceAPITests.Models.Responses.WalletService;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests.Scenarios.WalletService
{
    [TestFixture]
    public class GetTransactions : BaseWalletServiceTest
    {       
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
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
                Assert.IsNotEmpty(responseTransactions);
            });
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
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
                Assert.IsEmpty(responseTransactions);
            });
        }
        [Test]
        public async Task GetTransactions_TransactionStatus_CorrectStatus
            ([Values( 2, 1)] int expedctedTrtansactionStatus)
        {
           
            //Precondition
            int userId = await CreateAndVerifyUser();
            ChargeModel chargeModel = new ChargeModel
            {
                amount = 10,
                userId = userId,
            };
                //Make transaction
            var chargeResponse = await _walletServiceProvider.PostCharge(chargeModel);
            string transactionId = chargeResponse.Body;

            if (expedctedTrtansactionStatus ==2)            
                await _walletServiceProvider.RevertTransaction(transactionId);

            //Action
            var getTransactionsResponse = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> transactions = getTransactionsResponse.Body;
            GetTransactionModel transaction = transactions.FirstOrDefault(i => i.transactionId == transactionId);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(chargeModel.amount, transaction.amount);
                Assert.AreEqual(chargeModel.userId, transaction.userId);
                Assert.AreEqual(expedctedTrtansactionStatus, transaction.status);
            });

        }

        [Test]
        public async Task GetTransactions_NoTransactionsMade_CorrectStatus()
        {

            //Precondition
            int userId = await CreateAndVerifyUser();
            
            //Action
            var getTransactionsResponse = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> transactions = getTransactionsResponse.Body;

            //Assert
            Assert.AreEqual(transactions.Count,0);

        }

        [Test]
        public async Task GetTransactions_OneTransaction_CorrectStatus()
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
            //Action
            var getTransactionsResponse = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> transactions = getTransactionsResponse.Body;

            //Assert
            Assert.AreEqual(transactions.Count, 1);
        }
        
        [Test]
        public async Task GetTransactions_SeveralTransactions_CorrectStatus([Values(3, 30, 55)] int numberOfTransactions)
        {
            Random random = new Random();
            //Precondition
            int userId = await CreateAndVerifyUser();
            ChargeModel chargeModel = new ChargeModel();
            chargeModel.userId = userId;

            //Make transaction
            for (int i =1; i < numberOfTransactions; i++)
            {
                chargeModel.amount = random.NextDouble(1.00, 50000.00);
                await _walletServiceProvider.PostCharge(chargeModel);
            }
            await _walletServiceProvider.PostCharge(chargeModel);
            //Action
            var getTransactionsResponse = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> transactions = getTransactionsResponse.Body;

            //Assert
            Assert.AreEqual(numberOfTransactions, transactions.Count );
        }

        [Test]
        public async Task GetTransactions_CheckSortingByTime_CorrectOrder([Values(3, 30, 55)] int numberOfTransactions)
        {
            Random random = new Random();
            //Precondition
            int userId = await CreateAndVerifyUser();
            ChargeModel chargeModel = new ChargeModel();
            chargeModel.userId = userId;

            //Make transaction
            for (int i = 1; i < numberOfTransactions; i++)
            {
                chargeModel.amount = random.NextDouble(1.00, 50000.00);
                await _walletServiceProvider.PostCharge(chargeModel);
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
            }
            await _walletServiceProvider.PostCharge(chargeModel);
            //Action
            var getTransactionsResponse = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> transactions = getTransactionsResponse.Body;

            //Assert
            Assert.That(transactions, Is.Ordered.By("time").Descending);
        }

        [Test]
        public async Task GetTransactions_CheckAllFieldsAreNotNull_ValuesInRange([Values(3, 15, 20)] int numberOfTransactions)
        {
            Random random = new Random();
            //Precondition
            int userId = await CreateAndVerifyUser();
            ChargeModel chargeModel = new ChargeModel();
            chargeModel.userId = userId;

            //Make transaction
            for (int i = 1; i < numberOfTransactions; i++)
            {
                chargeModel.amount = random.NextDouble(0.01, 9999999.99);
                await _walletServiceProvider.PostCharge(chargeModel);
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
            }
            await _walletServiceProvider.PostCharge(chargeModel);
            //Action
            var getTransactionsResponse = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> transactions = getTransactionsResponse.Body;

            //Assert
            foreach(var transaction in transactions)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(transaction.amount, Is.GreaterThan(0));
                    Assert.That(transaction.time, Is.LessThanOrEqualTo(DateTime.Now));
                    Assert.That(transaction.status, Is.GreaterThan(-2));
                    Assert.That(transaction.baseTransactionId, Is.EqualTo(null));
                });
            }
        }

        [Test]
        public async Task GetTransactions_MakeGetTransactionsRequestAfterRevert_ValuesInRange()
        {
            Random random = new Random();
            //Precondition
            int userId = await CreateAndVerifyUser();
            ChargeModel chargeModel = new ChargeModel();
            chargeModel.userId = userId;
            chargeModel.amount = random.NextDouble(0.01, 9999999.99);
            
            var transactionResult = await _walletServiceProvider.PostCharge(chargeModel);
            
            Thread.Sleep(TimeSpan.FromSeconds(0.01));
            
            await _walletServiceProvider.RevertTransaction(transactionResult.Body);
            //Action

            var getTransactionsResponse = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> transactions = getTransactionsResponse.Body;

            //Assert
            GetTransactionModel firstTransaction = transactions[1];
            GetTransactionModel revertTransaction = transactions[0];

            Assert.Multiple(() =>
            {
                Assert.That(firstTransaction.amount, Is.GreaterThan(0));
                Assert.That(firstTransaction.time, Is.LessThanOrEqualTo(DateTime.Now));
                Assert.That(firstTransaction.status, Is.GreaterThan(-2));
                Assert.That(firstTransaction.baseTransactionId, Is.EqualTo(null));

                Assert.That(revertTransaction.amount, Is.GreaterThanOrEqualTo(-9999999.99));
                Assert.That(revertTransaction.time, Is.LessThanOrEqualTo(DateTime.Now));
                Assert.That(revertTransaction.status, Is.GreaterThan(-2));
                Assert.That(revertTransaction.baseTransactionId, Is.EqualTo(transactionResult.Body));            
            });
        }

        [Test]
        public async Task GetTransactions_TransactionWithPointZeroOne_CorrectStatus([Values(0.01, -0.01)] double transactionAmount)
        {
            Random random = new Random();
            //Precondition
            int userId = communUserId;
            ChargeModel chargeModel = new ChargeModel();
            chargeModel.userId = userId;
            chargeModel.amount = transactionAmount;

            var res = await _walletServiceProvider.PostCharge(chargeModel);
            
            //Action
            var getTransactionsResponse = await _walletServiceProvider.GetTransactions(userId);
            List<GetTransactionModel> transactions = getTransactionsResponse.Body;
            GetTransactionModel transaction = transactions.First();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(transaction.amount, Is.EqualTo(transactionAmount));
                Assert.That(transaction.time, Is.LessThanOrEqualTo(DateTime.Now));
                Assert.That(transaction.status, Is.GreaterThan(-2));
                Assert.That(transaction.baseTransactionId, Is.EqualTo(null));
            });
        }        
    }
}
