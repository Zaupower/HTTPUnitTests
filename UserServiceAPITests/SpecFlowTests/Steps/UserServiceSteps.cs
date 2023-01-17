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
        private UserServiceServiceProvider _userServiceProvider = UserServiceServiceProvider.Instance;
        
        [Given("User registration values are valid")]//Precondition
        public void CreateUser()
        {
            CreateUserRequest user = GenerateUsersRequest.Instance.generateUser();
            ScenarioContext.Current["userID"] = user;
        }
        
        [When("Register User")]//Action        
        public async Task RegisterUser()
        {
            HttpResponse<int> response = await _userServiceProvider.CreateUser((CreateUserRequest)ScenarioContext.Current["userID"]);
            ScenarioContext.Current["userCreatedResponse"] = response;
        }
    }
}
