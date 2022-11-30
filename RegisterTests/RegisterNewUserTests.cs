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
            
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.Greater( response.Body,0);
            Assert.AreEqual(response.Body.ToString(), response.Content);
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
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(0, response.Body);
            Assert.AreEqual("An error occurred while saving the entity changes. See the inner exception for details.", response.Content);
        }
    }
}
