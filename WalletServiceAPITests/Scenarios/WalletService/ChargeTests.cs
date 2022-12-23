using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Net;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.ServiceProvider;
using UserServiceAPITests.UserManagementTests;
using WalletServiceAPITests.Models.Requests.WalletService;
using WalletServiceAPITests.Models.Responses.Base;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests.Scenarios.WalletService
{
    public class ChargeTests
    {
        private WalletServiceServiceProvider _walletServiceProvider = WalletServiceServiceProvider.Instance;
        private UserServiceServiceProvider _userServiceProvider = UserServiceServiceProvider.Instance;


        private GenerateUsersRequest _generateUsersRequest = GenerateUsersRequest.Instance;

        private WalletServiceAPITests.TestDataObserver _observerCharge;
        private WalletServiceAPITests.TestDataObserverDeleteAction _observerRevert;

        private int communUserId;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _observerCharge = TestDataObserver.Instance;
            _observerRevert = TestDataObserverDeleteAction.Instance;

            _walletServiceProvider.Subscribe(_observerCharge);
            _walletServiceProvider.SubscribeRevert(_observerRevert);

            //Create common user
            CreateUserRequest request = _generateUsersRequest.generateUser();
            var createUserresponse = await _userServiceProvider.CreateUser(request);
            SetUserStatusModel setUserStatus = new SetUserStatusModel
            {
                UserId = createUserresponse.Body,
                NewStatus = true
            };
            await _userServiceProvider.SetUserStatus(setUserStatus);
            communUserId = createUserresponse.Body;
        }
        [OneTimeTearDown]
        public async Task OneTimeTearDown()
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
        [SetUp]
        public async Task SetUp()
        {            
        }

        [TearDown]
        public async Task TearDown()
        {
            
        }
        [Test]
        public async Task Charge_ValidUserValidAmount_FinalBalenceIs0()
        {
            //Precondition
            double[] amounts = { 50, -50, 100, -50, -50 };
            ChargeModel charge = new ChargeModel();
            charge.userId = communUserId;

            foreach (var amount in amounts)
            {
                charge.amount = amount;
                await _walletServiceProvider.PostCharge(charge);
            }
            //Action
            
            var respBalance = await _walletServiceProvider.GetBalance(communUserId);
            //Assert
            Assert.AreEqual(respBalance.Body, 0);
        }

        [Test]
        public async Task Charge_ValidUserValidAmount_FinalBalenceIsPointZeroOne()
        {
            //Precondition
            double[] amounts = { 50, -50, 100, -50, -49.9 };
            ChargeModel charge = new ChargeModel();
            charge.userId = communUserId;

            foreach (var amount in amounts)
            {
                charge.amount = amount;
                await _walletServiceProvider.PostCharge(charge);
            }
            //Action

            var respBalance = await _walletServiceProvider.GetBalance(communUserId);
            //Assert
            Assert.AreEqual(respBalance.Body, 0.1);
        }

        [Test]
        public async Task Charge_ValidUserValidAmount_FinalBalenceIsMinusPointZeroOne()
        {
            //Precondition
            double[] amounts = { 50, -50, 100, -100.1 };
            ChargeModel charge = new ChargeModel();
            charge.userId = communUserId;
            charge.amount = 0.1;
            var response = await _walletServiceProvider.PostCharge(charge);
            foreach (var amount in amounts)
            {
                charge.amount = amount;
                await _walletServiceProvider.PostCharge(charge);
            }

            //Revert first transaction
            await _walletServiceProvider.RevertTransaction(response.Body);

            ////Action
            var respBalance = await _walletServiceProvider.GetBalance(communUserId);
            //Assert
            Assert.AreEqual(-0.1, respBalance.Body);
        }

        [Test]
        public async Task Charge_ValidUserValidAmount_FinalBalenceIsBigNumber()
        {
            //Precondition
            double[] amounts = { 3333333.33, 3333333.33, -3333333.33, 3333333.33, 3333333.33 };
            ChargeModel charge = new ChargeModel();
            charge.userId = communUserId;
            foreach (var amount in amounts)
            {
                charge.amount = amount;
                await _walletServiceProvider.PostCharge(charge);
            }
            ////Action
            var respBalance = await _walletServiceProvider.GetBalance(communUserId);
            //Assert
            Assert.AreEqual(9999999.99, respBalance.Body);
        }

        [Test]
        public async Task Charge_ValidUserValidAmount_FinalBalenceIs10000000()
        {
            //Precondition
            double[] amounts = { 10000000, -10000000, 5000000, 2500000, 2500000 };
            ChargeModel charge = new ChargeModel();
            charge.userId = communUserId;
            foreach (var amount in amounts)
            {
                charge.amount = amount;
                await _walletServiceProvider.PostCharge(charge);
            }

            //Action
            var respBalance = await _walletServiceProvider.GetBalance(communUserId);
            //Assert
            Assert.AreEqual(10000000, respBalance.Body);
        }

        //ESTOU AQUI!!
        [Test]
        public async Task Charge_ValidUserValidAmount_FinalBalenceIsMinus10000000Point1()
        {
            //Precondition
            double[] amounts = { -1000000};
            ChargeModel charge = new ChargeModel();
            charge.userId = communUserId;
            charge.amount = 5000000;
            var firstTransaction = await _walletServiceProvider.PostCharge(charge);
            charge.amount = 5000000.01;
            var secondTransaction = await _walletServiceProvider.PostCharge(charge);
            foreach (var amount in amounts)
            {
                charge.amount = amount;
                await _walletServiceProvider.PostCharge(charge);
            }

            //Revert first transaction
            await _walletServiceProvider.RevertTransaction(firstTransaction.Body);
            await _walletServiceProvider.RevertTransaction(secondTransaction.Body);
            ////Action
            var respBalance = await _walletServiceProvider.GetBalance(communUserId);
            //Assert
            Assert.AreEqual(-10000000.01, respBalance.Body);
        }

        [Test]
        public async Task Charge_InvalidUserValidAmount_ReturnStatusIsOk([Values(0, 1)] int userId, [Values(50, -50, 0)] double amount)
        {
            double expectedBody = 0;
            //Precondition
            //Set user balence to 0 first

            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = userId,
            };
            //Action
            var response = await _walletServiceProvider.PostCharge(charge);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(response.Content, "not active user");
        }

        //If amount< 0 and there is less money in the account than amount =>
        //Code:  500; Message  “User has '30', you try to charge '-40'.”
        [Test]
        public async Task Charge_ValidUserAmountBelowCurrentAccAmount_ReturnStatusIsInternalServerError
            ([Values(3)] int userId, [Values(10)] double balance, [Values(-20)] double amount)
        {
            string expectedContent = "User have '" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", balance) + "', you try to charge '" + String.Format(CultureInfo.InvariantCulture, "{0:0.0}", amount ) + "'.";
            //Precondition
            await SetBalance(userId, balance);
            var currentBalanceResponse = await _walletServiceProvider.GetBalance(userId);
            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = userId,
            };
            //Action
            var response = await _walletServiceProvider.PostCharge(charge);
            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(expectedContent, response.Content);
        }
        //If the balance is <0 and the amount is <0 => Code:  500;
        //Message “User has '-30', you try to charge '-40'.”
        [Test]
        public async Task Charge_ValidUserBalanceBelowZero_ReturnStatusIsInternalServerError
            ( [Values(-10)] double balance, [Values(-40)] double amount)
        {
            string expectedContent = "User have '" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", balance) + "', you try to charge '" + String.Format(CultureInfo.InvariantCulture, "{0:0.0}", amount) + "'.";
            //Precondition
            await SetBalance(communUserId, balance);
            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = communUserId,
            };
            //Action
            var response = await _walletServiceProvider.PostCharge(charge);
            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(expectedContent, response.Content);
        }
        //If the amount = 0 => Code:  500; Message “Amount cannot be '0'”
        [Test]
        public async Task Charge_ValidUserChargeZero_ReturnStatusIsInternalServerError
            ( [Values(0)] double amount)
        {
            string expectedContent = "Amount cannot be '0'";
            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = communUserId,
            };
            //Action
            var response = await _walletServiceProvider.PostCharge(charge);
            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(expectedContent, response.Content);
        }
        //If the amount + balance > 10 000 000=> Code:  500;
        //Message  “After this charge balance could be '{amount + balance}', maximum user balance is '10000000'”
        [Test]
        public async Task Charge_ValidUserMaximumUserBalance_ReturnStatusIsInternalServerError
            ([Values(-10, 0, 500)] double balance, [Values(10000012, 10002001, 19000501)] double amount)
        {
            
            string expectedContent = $"After this charge balance could be '"+ String.Format(CultureInfo.InvariantCulture,"{0:0.00}", amount + balance) + "', maximum user balance is '10000000'";
            //Precondition
            await SetBalance(communUserId, balance);
            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = communUserId,
            };
            //Action
            var response = await _walletServiceProvider.PostCharge(charge);
            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(expectedContent, response.Content);
        }

        //If the write-off amount after the decimal point has more than two decimal places => Code:  500;
        //Message “ Amount value must have precision 2 numbers after dot”
        [Test]
        public async Task Charge_ValidUserMoreThanTwoDecimalPlaces_ReturnStatusIsInternalServerError
            ([Values(10, -10)] double balance, [Values(5.008)] double amount)
        {
            string expectedContent = "Amount value must have precision 2 numbers after dot";
            //Precondition
            await SetBalance(communUserId, balance);
            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = communUserId,
            };
            //Action
            var response = await _walletServiceProvider.PostCharge(charge);
            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(expectedContent, response.Content);
        }


        #region Helper Functions        
        private async Task SetBalance(int userId, double inputBalance)
        {
            double actualBalance;
            var currentBalanceResponse = await _walletServiceProvider.GetBalance(userId);
            actualBalance = currentBalanceResponse.Body;
            ChargeModel chargeModel = new ChargeModel
            {
                amount = -actualBalance,
                userId = userId,
            };

            if (actualBalance != 0)
            {
                chargeModel = new ChargeModel
                {
                    amount = -actualBalance,
                    userId = userId,

                };
                //Set Balance to 0
                await _walletServiceProvider.PostCharge(chargeModel);
            }
                
            if (inputBalance >= 0)
            {
                chargeModel.amount = inputBalance;
                //Add Balance
                var res = await _walletServiceProvider.PostCharge(chargeModel);
            }
            else
            {
                chargeModel.amount = -inputBalance;
                var resPostCharge = await _walletServiceProvider.PostCharge(chargeModel);
                //Charge -20
                chargeModel.amount = inputBalance;
                await _walletServiceProvider.PostCharge(chargeModel);
                //Cancel (Add 30)
                var result = await _walletServiceProvider.RevertTransaction(resPostCharge.Body);

                var currentBalanceResponseAfterNegativeS = await _walletServiceProvider.GetBalance(userId);
            }
        }

        
        #endregion


    }
}
