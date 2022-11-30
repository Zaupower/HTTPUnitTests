using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests;
using UserServiceAPITests.Models.Responses.Base;

namespace UserServiceAPITests.RegisterTests
{
    public class DeleteUserTests
    {
        private UserServiceServiceProvider _serviceProvider = new UserServiceServiceProvider();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ValidUser_DeleteUser_ResponseStatusIsOk()
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrt_name_test1",
                lastName = "last_name_test1"
            };

            HttpResponse<int> createUserResponse = await _serviceProvider.CreateUser(request);
            int userId = createUserResponse.Body;

            //Action
            var deleteResponse = await _serviceProvider.DeleteUser(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.HttpStatusCode);
            Assert.AreEqual(null, deleteResponse.Body);
            Assert.AreEqual("", deleteResponse.Content);
        }

        [Test]
        public async Task ValidUser_DeleteUserTwoTimes_ResponseStatusIsStatusInternalServerError()
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrt_name_test1",
                lastName = "last_name_test1"
            };

            HttpResponse<int> createUserResponse = await _serviceProvider.CreateUser(request);
            int userId = createUserResponse.Body;

            //Action
                //Delete first time
            await _serviceProvider.DeleteUser(userId);
                //Delete second time
            var deleteResponse = await _serviceProvider.DeleteUser(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, deleteResponse.HttpStatusCode);
            Assert.AreEqual(deleteResponse.Content, "Sequence contains no elements");
            Assert.AreEqual(null, deleteResponse.Body);
        }

        [Test]
        public async Task InvalidUser_DeleteUser_ResponseStatusIsInternalServerError()
        {
            //Precondiction
            int userId = 0;

            //Action
            var deleteResponse = await _serviceProvider.DeleteUser(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, deleteResponse.HttpStatusCode);
            Assert.AreEqual(deleteResponse.Content, "Sequence contains no elements");
            Assert.AreEqual(null, deleteResponse.Body);
        }
    }
}
