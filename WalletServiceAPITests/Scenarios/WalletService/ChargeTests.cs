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
            _observer = TestDataObserver.Instance;
            _serviceProvider.Subscribe(_observer);
        }
        [OneTimeTearDown]
        public async Task teardown()
        {
            foreach(var transactionMade in _observer.GetAll())
            {
                await _serviceProvider.RevertTransaction(transactionMade);
            }

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
            string expectedContent = "User have '" + string.Format(GetNfi(".", 2), "{0:N}", balance) + "', you try to charge '" + string.Format(GetNfi(".", 1), "{0:N}", amount) + "'.";
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
            string expectedContent = "User have '" + string.Format(GetNfi(".", 2), "{0:N}", balance) + "', you try to charge '" + string.Format(GetNfi(".", 1), "{0:N}", amount) + "'.";
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


        #region Helper Functions
        private static NumberFormatInfo GetNfi(string separator, int numberOfDecimalDigits)
        {
            NumberFormatInfo nfiBalance = new NumberFormatInfo();
            nfiBalance.NumberDecimalSeparator = separator;
            nfiBalance.NumberDecimalDigits = numberOfDecimalDigits;
            return nfiBalance;
        }

        private async Task SetBalance(int userId, double balance)
        {
            double actualBalance = 0;

            if (balance >= 0)
            {
                
                //Precondition
                var currentBalanceResponse = await _serviceProvider.GetBalance(userId);
                actualBalance = currentBalanceResponse.Body;

                ChargeModel netZeroCharge = new ChargeModel
                {
                    amount = -actualBalance,
                    userId = userId,

                };
                //Set Balance to 0
                await _serviceProvider.PostCharge(netZeroCharge);
                netZeroCharge.amount = balance;
                //Add Balance
                var res = await _serviceProvider.PostCharge(netZeroCharge);

            }
            else
            {
                var currentBalanceResponse = await _serviceProvider.GetBalance(userId);
                actualBalance = currentBalanceResponse.Body;

                ChargeModel charge = new ChargeModel
                {
                    amount = -actualBalance,
                    userId = userId,

                };
                //Set Balance to 0
                await _serviceProvider.PostCharge(charge);

                //Add 30
                charge.amount = balance;
                var resPostCharge = await _serviceProvider.PostCharge(charge);
                //Charge -20
                charge.amount = -balance;
                await _serviceProvider.PostCharge(charge);
                //Cancel (Add 30)
                var result = await _serviceProvider.RevertTransaction(resPostCharge.Body);

                var currentBalanceResponseAfterNegativeS = await _serviceProvider.GetBalance(userId);
            }
            Console.WriteLine("fdfd");
        }

        
        #endregion


    }
}
