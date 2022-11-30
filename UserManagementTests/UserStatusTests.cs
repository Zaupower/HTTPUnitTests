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
    public class UserStatusTests
    {

        private UserServiceServiceProvider _serviceProvider = new UserServiceServiceProvider();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ValidUser_SetUserStatus_ResponseStatusIsOk()
        {
            //Precondition
            bool newUserStatus = true;
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
        }
    }
}
