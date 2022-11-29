using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models;

namespace UserServiceAPITests.RegisterTests
{
    
    public class RegisterNewUserTests
    {
        

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ValidUSer_PerformRequest_ResponseStatusIsOk()
        {
            
            UserServiceServiceProvider serviceProvider = new UserServiceServiceProvider();

            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrt_name_test1",
                lastName = "last_name_test1"
            };

            HttpStatusCode responseStatus = await serviceProvider.CreateUser(request);

            ////

            ////GetUserResponse createUserResponse = new GetUserResponse 
            ////{ 
            ////    id = int.Parse(responseContent),
            ////};

            //////Assert

            //////ID is int greater than 0
            ////Assert.Greater(createUserResponse.id, 0);

            //Status Code is OK
            Assert.AreEqual(HttpStatusCode.OK, responseStatus);

        }
    }
}
