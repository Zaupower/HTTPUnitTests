using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public async Task Charge_ValidUserValidAmount_ReturnStatusIsOk([Values(5)] int userId)
        {
            //Precondition
            //Positive chage add money to user acc
            //Negative chage subtract money to user acc
            ChargeModel charge = new ChargeModel
            {
                amount = -9.3,
                userId = userId,
                           
            };
            //Action
            var response = await _serviceProvider.PostCharge(charge);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.IsTrue(Guid.TryParse(response.Body, out _));
            //Assert.AreEqual(response.Content, "not active user");
            //After chage should reverse transaction
        }
    }
}
