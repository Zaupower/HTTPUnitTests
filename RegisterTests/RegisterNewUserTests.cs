using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests;
using UserServiceAPITests.Models.Responses;
using UserServiceAPITests.Models.Responses.Base;

namespace UserServiceAPITests.RegisterTests
{

    public class RegisterNewUserTests
    {
        private UserServiceServiceProvider _serviceProvider = new UserServiceServiceProvider();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ValidUser_RegisterNewUSer_ResponseStatusIsOk()
        {
            
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrtNameTest1",
                lastName = "lastNameTest1"
            };

            //Action
            HttpResponse<int> response = await _serviceProvider.CreateUser(request);
            
            Assert.Greater( response.Body,0);
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
        }

        [Test]
        public async Task InvalidUser_RegisterNewUSer_ResponseStatusIsOk()
        {

            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = null,
                lastName = null
            };

            //Action
            HttpResponse<int> response = await _serviceProvider.CreateUser(request);

            //Assert
            Assert.AreEqual(response.Content, "An error occurred while saving the entity changes. See the inner exception for details.");
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
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
            int userId =  createUserResponse.Body;
            
            //Action
            var deleteResponse = await _serviceProvider.DeleteUser(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.HttpStatusCode);
        }

        [Test]
        public async Task InvalidUser_DeleteUser_ResponseStatusInternalServerError()
        {
            //Precondiction
            int userId = 0;
            
            //Action
            var deleteResponse = await _serviceProvider.DeleteUser(userId);

            //Assert
            Assert.AreEqual(deleteResponse.Content, "Sequence contains no elements");
            Assert.AreEqual(HttpStatusCode.InternalServerError, deleteResponse.HttpStatusCode);
        }
    }
}
