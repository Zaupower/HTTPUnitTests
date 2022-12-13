using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Globalization;
using System.Net;
using WalletServiceAPITests.Models.Requests.WalletService;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests.Scenarios.WalletService
{
    public class ChargeTests
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
            foreach(var transactionMade in _observer.GetAll())
            {
                await _serviceProvider.RevertTransaction(transactionMade);
            }
            _observer.OnCompleted();
        }

        [Test]
        public async Task Charge_ValidUserValidAmount_ReturnStatusIsOk([Values(5)] int userId, [Values(50, -50)] double amount)
        {
            //Precondition
            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = userId,
                           
            };
            //Action
            var response = await _serviceProvider.PostCharge(charge);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.IsTrue(Guid.TryParse(response.Body, out _));
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
            var response = await _serviceProvider.PostCharge(charge);

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
            var currentBalanceResponse = await _serviceProvider.GetBalance(userId);
            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = userId,
            };
            //Action
            var response = await _serviceProvider.PostCharge(charge);
            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(expectedContent, response.Content);
        }
        //If the balance is <0 and the amount is <0 => Code:  500;
        //Message “User has '-30', you try to charge '-40'.”
        [Test]
        public async Task Charge_ValidUserBalanceBelowZero_ReturnStatusIsInternalServerError
            ([Values(3)] int userId, [Values(-10)] double balance, [Values(-40)] double amount)
        {
            string expectedContent = "User have '" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", balance) + "', you try to charge '" + String.Format(CultureInfo.InvariantCulture, "{0:0.0}", amount) + "'.";
            //Precondition
            await SetBalance(userId, balance);
            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = userId,
            };
            //Action
            var response = await _serviceProvider.PostCharge(charge);
            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(expectedContent, response.Content);
        }
        //If the amount = 0 => Code:  500; Message “Amount cannot be '0'”
        [Test]
        public async Task Charge_ValidUserChargeZero_ReturnStatusIsInternalServerError
            ([Values(3)] int userId, [Values(0)] double amount)
        {
            string expectedContent = "Amount cannot be '0'";
            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = userId,
            };
            //Action
            var response = await _serviceProvider.PostCharge(charge);
            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(expectedContent, response.Content);
        }
        //If the amount + balance > 10 000 000=> Code:  500;
        //Message  “After this charge balance could be '{amount + balance}', maximum user balance is '10000000'”
        [Test]
        public async Task Charge_ValidUserMaximumUserBalance_ReturnStatusIsInternalServerError
            ([Values(3)] int userId, [Values(-10, 0, 500)] double balance, [Values(10000012, 10002001, 19000501)] double amount)
        {
            
            string expectedContent = $"After this charge balance could be '"+ String.Format(CultureInfo.InvariantCulture,"{0:0.00}", amount + balance) + "', maximum user balance is '10000000'";
            //Precondition
            await SetBalance(userId, balance);
            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = userId,
            };
            //Action
            var response = await _serviceProvider.PostCharge(charge);
            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(expectedContent, response.Content);
        }

        //If the write-off amount after the decimal point has more than two decimal places => Code:  500;
        //Message “ Amount value must have precision 2 numbers after dot”
        [Test]
        public async Task Charge_ValidUserMoreThanTwoDecimalPlaces_ReturnStatusIsInternalServerError
            ([Values(3)] int userId, [Values(10, -10)] double balance, [Values(5.008)] double amount)
        {
            string expectedContent = "Amount value must have precision 2 numbers after dot";
            //Precondition
            await SetBalance(userId, balance);
            ChargeModel charge = new ChargeModel
            {
                amount = amount,
                userId = userId,
            };
            //Action
            var response = await _serviceProvider.PostCharge(charge);
            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(expectedContent, response.Content);
        }


        #region Helper Functions
        
        private async Task SetBalance(int userId, double inputBalance)
        {
            double actualBalance;
            var currentBalanceResponse = await _serviceProvider.GetBalance(userId);
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
                await _serviceProvider.PostCharge(chargeModel);
            }
                
            if (inputBalance >= 0)
            {
                chargeModel.amount = inputBalance;
                //Add Balance
                var res = await _serviceProvider.PostCharge(chargeModel);
            }
            else
            {
                chargeModel.amount = -inputBalance;
                var resPostCharge = await _serviceProvider.PostCharge(chargeModel, true);
                //Charge -20
                chargeModel.amount = inputBalance;
                await _serviceProvider.PostCharge(chargeModel);
                //Cancel (Add 30)
                var result = await _serviceProvider.RevertTransaction(resPostCharge.Body);

                var currentBalanceResponseAfterNegativeS = await _serviceProvider.GetBalance(userId);
            }
        }

        
        #endregion


    }
}
