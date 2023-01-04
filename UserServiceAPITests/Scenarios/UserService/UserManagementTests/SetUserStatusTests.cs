using CommonLogic.Models.Responses.Base;
using NUnit.Framework;
using System.Net;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Scenarios;

namespace UserServiceAPITests.UserManagementTests
{
    [TestFixture]
    public class UserStatusTests : UserServiceBaseTest
    {
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
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, setUserStatusResponse.HttpStatusCode);
                Assert.AreEqual(null, setUserStatusResponse.Body);
                Assert.AreEqual("", setUserStatusResponse.Content);
            });
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
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, setUserStatusResponse.HttpStatusCode);
                Assert.AreEqual(null, setUserStatusResponse.Body);
                Assert.AreEqual(setUserStatusResponse.Content, "Sequence contains no elements");
            });
        }
        
        [Test]
        public async Task ValidUser_SetUserStatusFalseTrueFalse_ResponseStatusIsOk()
        {
            bool newUserStatus = false;
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
            await _serviceProvider.SetUserStatus(setUserStatus);
            //true
            setUserStatus.NewStatus = true;
            await _serviceProvider.SetUserStatus(setUserStatus);
            //false
            //Action 
            setUserStatus.NewStatus = false;            
            HttpResponse<object> setUserStatusResponse = await _serviceProvider.SetUserStatus(setUserStatus);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, setUserStatusResponse.HttpStatusCode);
                Assert.AreEqual(null, setUserStatusResponse.Body);
                Assert.AreEqual("", setUserStatusResponse.Content);
            });
        }


    }
}
