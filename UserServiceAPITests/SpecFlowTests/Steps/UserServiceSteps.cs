using CommonLogic.Models.Responses.Base;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Net;
using TechTalk.SpecFlow;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.SpecFlowTests.Steps
{
    [Binding]
    public class UserServiceSteps
    {
        private CreateUserRequest user;
        private HttpResponse<int> response;
        private UserServiceServiceProvider _serviceProvider = UserServiceServiceProvider.Instance;
        
        [Given("User registration values are valid")]//Precondition
        public void CreateUser()
        {
            user = GenerateUsersRequest.Instance.generateUser();
        }

        [When("Register User")]//Action        
        public async Task RegisterUser()
        {
            response = await _serviceProvider.CreateUser(user);
            //Assert
        }

        [Then("Create User Request Status is '(.*)'")]//Assert
        public void ThenCreateUserRequestStatusIsOK(HttpStatusCode expectedStatusCode)
        {
            Assert.AreEqual(expectedStatusCode, response.HttpStatusCode);
        }

    }
}
