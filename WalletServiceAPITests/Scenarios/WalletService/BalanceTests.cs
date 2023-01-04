using NUnit.Framework;
using System;
using System.Net;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.ServiceProvider;
using WalletServiceAPITests.Models.Responses.Base;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests.Scenarios.WalletService
{
    
    [TestFixture]
    public class BalanceTests : BaseWalletServiceTest
    {
        
        [Test]
        public async Task GetBalance_VerifiedUser_ResponseStatusIsOk()
        {
            //Precondition

            //Action
            var response = await _walletServiceProvider.GetBalance(communUserId);            
            
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);            
            Assert.IsTrue(response.Body is double);       
        }

        [Test]
        public async Task GetBalance_UnverifiedAndInvalidUser_ResponseStatusIsOk([Values(0,1)] int userId)
        {
            //Precondition
            double expectedBody = 0;
            //Action
            var response = await _walletServiceProvider.GetBalance(userId);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
                Assert.AreEqual(expectedBody, response.Body);
                Assert.AreEqual(response.Content, "not active user");
            });
        }

        [Test]
        public async Task GetBalance_NewUser_ResponseStatusIsOk()
        {
            //Precondition
            double expectedBody = 0;
            CreateUserRequest newUSer = _generateUsersRequest.generateUser();

            var createUserresponse = await _userServiceProvider.CreateUser(newUSer);

            int userId = createUserresponse.Body;
            //Action
            var response = await _walletServiceProvider.GetBalance(userId);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
                Assert.AreEqual(expectedBody, response.Body);
                Assert.AreEqual(response.Content, "not active user");
            });
        }
    }
}
