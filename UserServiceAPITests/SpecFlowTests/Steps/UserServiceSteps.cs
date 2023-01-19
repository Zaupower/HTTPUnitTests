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
        private UserServiceServiceProvider _userServiceProvider;
        private DataContext _context;
        public UserServiceSteps(DataContext context, UserServiceServiceProvider userServiceProvider)
        {
            _context = context;
            _userServiceProvider = userServiceProvider;
        }

        [Given("User registration values are valid")]//Precondition
        public void CreateUser()
        {
            CreateUserRequest user = GenerateUsersRequest.Instance.generateUser();
            _context.CreateUserRequest = user;
        }
        
        [When("Register User")]//Action        
        public async Task RegisterUser()
        {
            HttpResponse<int> response = await _userServiceProvider.CreateUser(_context.CreateUserRequest);
            _context.CreateUserResponse = response;
        }
    }
}
