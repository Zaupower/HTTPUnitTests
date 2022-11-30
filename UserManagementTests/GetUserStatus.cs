using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests;
using UserServiceAPITests.Models.Responses.Base;

namespace UserServiceAPITests.UserManagementTests
{
    public class GetUserStatus
    {
        private UserServiceServiceProvider _serviceProvider = new();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ValidUser_GetDefaultUserStatus_ResponseStatusIsOk()
        {
            //Precondition
            
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrtNameTest1",
                lastName = "lastNameTest1"
            };

            HttpResponse<int> createUserresponse = await _serviceProvider.CreateUser(request);
            int userId = createUserresponse.Body;

            //Action 
            HttpResponse<object> getUserStatusResponse = await _serviceProvider.GetUserStatus(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, getUserStatusResponse.HttpStatusCode);
            Assert.AreEqual(false, getUserStatusResponse.Body);
            Assert.AreEqual("false", getUserStatusResponse.Content);
        }

        [Test]
        public async Task ValidUser_GetUserStatusTrue_ResponseStatusIsOk()
        {
            //Precondition
            bool newUserStatus = true;
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrtNameTest1",
                lastName = "lastNameTest1"
            };

            HttpResponse<int> createUserresponse = await _serviceProvider.CreateUser(request);
            int userId = createUserresponse.Body;

            SetUserStatusModel setUserStatus = new SetUserStatusModel
            {
                UserId = createUserresponse.Body,
                NewStatus = newUserStatus
            };
            
            await _serviceProvider.SetUserStatus(setUserStatus);

            //Action 
            HttpResponse<object> getUserStatusResponse = await _serviceProvider.GetUserStatus(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, getUserStatusResponse.HttpStatusCode);
            Assert.AreEqual(true, getUserStatusResponse.Body);
            Assert.AreEqual("true", getUserStatusResponse.Content);
        }

        [Test]
        public async Task ValidUser_GetUserStatusFalse_ResponseStatusIsOk()
        {
            //Precondition
            bool newUserStatus = false;
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrtNameTest1",
                lastName = "lastNameTest1"
            };

            HttpResponse<int> createUserresponse = await _serviceProvider.CreateUser(request);
            int userId = createUserresponse.Body;

            SetUserStatusModel setUserStatus = new SetUserStatusModel
            {
                UserId = createUserresponse.Body,
                NewStatus = newUserStatus
            };
            
            await _serviceProvider.SetUserStatus(setUserStatus);

            //Action 
            HttpResponse<object> getUserStatusResponse = await _serviceProvider.GetUserStatus(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, getUserStatusResponse.HttpStatusCode);
            Assert.AreEqual(false, getUserStatusResponse.Body);
            Assert.AreEqual("false", getUserStatusResponse.Content);
        }
    }
}
