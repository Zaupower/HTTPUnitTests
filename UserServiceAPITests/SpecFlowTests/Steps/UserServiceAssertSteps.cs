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
    public class UserServiceAssertSteps
    {
        //private UserServiceServiceProvider _userServiceProvider = UserServiceServiceProvider.Instance;

        private DataContext _context;
        public UserServiceAssertSteps(DataContext context)
        {
            _context = context;
        }

        [Then("Create User Request Status is '(.*)'")]//Assert
        public void ThenCreateUserRequestStatusIsOK(HttpStatusCode expectedStatusCode)
        {
            HttpResponse<int> response = _context.CreateUserResponse;
            Assert.AreEqual(expectedStatusCode, response.HttpStatusCode);
        }

    }
}
