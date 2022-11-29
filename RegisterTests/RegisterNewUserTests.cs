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
                firstName = "fisrtNameTest1",
                lastName = "lastNameTest1"
            };

            HttpResponse<int> response = await _serviceProvider.CreateUser(request);
            
            //Assert.Greater(int.Parse( response.Body.id), 0);
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

        }

        [Test]
        public async Task ValidUSer_DeleteUser_ResponseStatusIsOk()
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
            //Assert.Greater(createUserResponse.id, 0);
            //Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.HttpStatusCode);

        }
    }
}
