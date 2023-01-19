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
        private ScenarioContext _context;
        public UserServiceSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given("User registration values are valid")]//Precondition
        public void CreateUser()
        {
            CreateUserRequest user = GenerateUsersRequest.Instance.generateUser();
            _context["userID"] = user;
        }
        
        [When("Register User")]//Action        
        public async Task RegisterUser()
        {
            HttpResponse<int> response = await _userServiceProvider.CreateUser((CreateUserRequest)_context["userID"]);
            _context["userCreatedResponse"] = response;
        }
    }
}
