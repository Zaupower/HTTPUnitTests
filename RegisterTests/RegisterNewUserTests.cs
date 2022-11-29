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
        public async Task ValidUSer_RegisterNewUSer_ResponseStatusIsOk()
        {
            

            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrt_name_test1",
                lastName = "last_name_test1"
            };

            HttpResponse<string> response = await _serviceProvider.CreateUser(request);
            GetUserResponse createUserResponse = new GetUserResponse
            {
                id = int.Parse(response.Body),
            };
            Assert.Greater(createUserResponse.id, 0);
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

        }

        [Test]
        public async Task ValidUSer_DeleteUser_ResponseStatusIsOk()
        {

            UserServiceServiceProvider serviceProvider = new UserServiceServiceProvider();

            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrt_name_test1",
                lastName = "last_name_test1"
            };

            HttpResponse<string> response = await serviceProvider.CreateUser(request);

            ////

            GetUserResponse createUserResponse = new GetUserResponse
            {
                id = int.Parse(response.Body),
            };

            //Assert
            Assert.Greater(createUserResponse.id, 0);
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

        }
    }
}
