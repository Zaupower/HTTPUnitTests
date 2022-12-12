using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.UserManagementTests
{
    public class UserStatusTests
    {

        private UserServiceServiceProvider _serviceProvider = UserServiceServiceProvider.Instance;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ValidUser_SetUserStatusTrue_ResponseStatusIsOk([Values(true, false)] bool newUserStatus)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrtNameTest1",
                lastName = "lastNameTest1"
            };

            HttpResponse<int> createUserresponse = await _serviceProvider.CreateUser(request);
            SetUserStatusModel setUserStatus = new SetUserStatusModel
            {
                UserId = createUserresponse.Body,
                NewStatus = newUserStatus
            };

            //Action 
            HttpResponse<object> setUserStatusResponse = await _serviceProvider.SetUserStatus(setUserStatus);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, setUserStatusResponse.HttpStatusCode);
            Assert.AreEqual(null, setUserStatusResponse.Body);
            Assert.AreEqual("", setUserStatusResponse.Content);
        }

        [Test]
        public async Task InvalidUser_SetUserStatusTrue_ResponseStatusIsInternalServerError([Values(true, false)] bool newUserStatus)
        {
            //Precondition
            int userId = 0;
            SetUserStatusModel setUserStatus = new SetUserStatusModel
            {
                UserId = userId,
                NewStatus = newUserStatus
            };

            //Action 
            HttpResponse<object> setUserStatusResponse = await _serviceProvider.SetUserStatus(setUserStatus);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, setUserStatusResponse.HttpStatusCode);
            Assert.AreEqual(null, setUserStatusResponse.Body);
            Assert.AreEqual(setUserStatusResponse.Content, "Sequence contains no elements");
        }
    }
}
