using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Models.Responses;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.RegisterTests
{

    public class RegisterNewUserTests
    {
        private UserServiceServiceProvider _serviceProvider = UserServiceServiceProvider.Instance;
        private GenerateUsersRequest _generateUsersRequest = GenerateUsersRequest.Instance;

        private TestDataObserver _observerNewUser;
        private TestDataObserverDeleteAction _observerDeleteUSer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _observerNewUser = TestDataObserver.Instance;
            _observerDeleteUSer = TestDataObserverDeleteAction.Instance;

            _serviceProvider.Subscribe(_observerNewUser);
            _serviceProvider.SubscribeDeleteUser(_observerDeleteUSer);
        }
        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            List<int> newUsers = _observerNewUser.GetAll().ToList();
            List<int> deletedUsers = _observerDeleteUSer.GetAll().ToList();

            List<int> resultList = newUsers.Except(deletedUsers).ToList();

            foreach (var userCreated in resultList)
            {
                await _serviceProvider.DeleteUser(userCreated);
            }
            _observerNewUser.OnCompleted();
            _observerDeleteUSer.OnCompleted();
        }
        [Test]
        public async Task ValidUser_RegisterNewUSer_ResponseStatusIsOk([Values(0, 1, 7, 15)] int numberOfUsers)
        {
            //Precondition
            List<CreateUserRequest> requestUsers = _generateUsersRequest.generateUsers(numberOfUsers);

            foreach (CreateUserRequest requestUser in requestUsers)
            {
                //Action
                HttpResponse<int> response = await _serviceProvider.CreateUser(requestUser);
                //Assert
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
                Assert.Greater(response.Body, 0);
                Assert.AreEqual(response.Body.ToString(), response.Content);

            }                        
        }

        [Test, Combinatorial]
        public async Task InvalidUser_RegisterNewUserFirstNameAllwaysNULL_ResponseStatusIsInternalServerError(
            [Values(null)] string firstName,
            [Values("TestCombinatorialLasttName", null )] string lastName)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = firstName,
                lastName = lastName
            };

            //Action
            HttpResponse<int> response = await _serviceProvider.CreateUser(request);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(0, response.Body);
            Assert.AreEqual("An error occurred while saving the entity changes. See the inner exception for details.", response.Content);
        }

        [Test, Combinatorial]
        public async Task InvalidUser_RegisterNewUserLastNameAllwaysNULL_ResponseStatusIsInternalServerError(
            [Values("TestCombinatorialFirstName", null)] string firstName,
            [Values( null)] string lastName)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = firstName,
                lastName = lastName
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
