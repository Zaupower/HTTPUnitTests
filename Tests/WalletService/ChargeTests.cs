using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests.WalletService;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.Tests.WalletService
{
    public class ChargeTests
    {
        private WalletServiceServiceProvider _serviceProvider = new();

        [SetUp]
        public void setup()
        {

        }
        [TearDown]
        public void teardown()
        {

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
            await SetPositiveBalance(userId, balance);
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
            await SetNegativeBalance(userId, balance);
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

        private async Task SetPositiveBalance(int userId, double balance)
        {
            double expectedBody = 0;
            double actualBalance = 0;
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
            Console.WriteLine("fdfd");
        }

        private async Task SetNegativeBalance(int userId, double balance)
        {
            double actualBalance = 0;
            //Precondition
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
            Console.WriteLine("fdfd");
        }
        #endregion


    }
}
