using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.UserManagementTests
{
    public class GetUserStatus
    {
        private UserServiceServiceProvider _serviceProvider = UserServiceServiceProvider.Instance;
        private GenerateUsersRequest _generateUsersRequest = GenerateUsersRequest.Instance;

        private TestDataObserver _observer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _observer = TestDataObserver.Instance;
            _serviceProvider.Subscribe(_observer);
        }
        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            foreach (var userCreated in _observer.GetAll())
            {
                await _serviceProvider.DeleteUser(userCreated);
            }
            _observer.OnCompleted();
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
            StringAssert.AreEqualIgnoringCase(getUserStatusResponse.Body.ToString(), getUserStatusResponse.Content);
        }

        [Test]
        public async Task ValidUser_GetUserStatus_ResponseStatusIsOk([Values(true, false)] bool newUserStatus)
        {
            //Precondition
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
            Assert.AreEqual(newUserStatus, getUserStatusResponse.Body);
            StringAssert.AreEqualIgnoringCase(getUserStatusResponse.Body.ToString(), getUserStatusResponse.Content);
        }

        [Test]
        public async Task InvalidUser_GetUserStatus_ResponseStatusIsInternalServerError([Values(false)] bool newUserStatus)
        {
            //Precondition
            
            int userId = 0;

            SetUserStatusModel setUserStatus = new SetUserStatusModel
            {
                UserId = userId,
                NewStatus = newUserStatus
            };

            await _serviceProvider.SetUserStatus(setUserStatus);

            //Action 
            HttpResponse<object> getUserStatusResponse = await _serviceProvider.GetUserStatus(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, getUserStatusResponse.HttpStatusCode);
            Assert.AreEqual(null, getUserStatusResponse.Body);
            Assert.AreEqual("Sequence contains no elements", getUserStatusResponse.Content);
        }
    }
}
