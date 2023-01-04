using CommonLogic.Models.Responses.Base;
using NUnit.Framework;
using System.Net;
using UserServiceAPITests.Models.Requests.UserService;
using WalletServiceAPITests.Models.Requests.WalletService;

namespace WalletServiceAPITests.Scenarios.WalletService
{
    [TestFixture]
    public class RevertTransaction : BaseWalletServiceTest
    {       
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
            var chargeResponse = await _walletServiceProvider.PostCharge(charge);
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
