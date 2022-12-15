using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.Models.Responses.UserService;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.Tests.CacheManagementTests
{
    public class CacheManagementRegisterUserTests
    {
        private UserServiceServiceProvider _serviceProvider = UserServiceServiceProvider.Instance;
        private GenerateUsersRequest _generateUsersRequest= GenerateUsersRequest.Instance;
        
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
        public async Task GetCacheManagement_CreateUser_ResponseStatusIsOk([Values(0, 1, 7, 15)] int numberOfUsers)
        {

            //Precondition
                //Delete current cache
            await _serviceProvider.DeleteCacheManagement();

            List<CreateUserRequest> requestUsers = _generateUsersRequest.generateUsers(numberOfUsers);

            foreach (CreateUserRequest request in requestUsers)
            {
                await _serviceProvider.CreateUser(request);
            }            

            //Action
            HttpResponse<List<GetUserResponse>> response = await _serviceProvider.GetCacheManagement();
            List<GetUserResponse> cacheUsers = response.Body;

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            cacheUsers.Should().BeEquivalentTo(requestUsers);//Body
        }

        [Test]
        public async Task DeleteCacheManagement_CreateUser_ResponseStatusIsOk([Values(0, 1, 7, 15)] int numberOfUsers)
        {

            //Precondition

            List<CreateUserRequest> requestUsers = _generateUsersRequest.generateUsers(numberOfUsers);

            foreach (CreateUserRequest request in requestUsers)
            {
                await _serviceProvider.CreateUser(request);
            }

            //Action
                //Delete current cache
            await _serviceProvider.DeleteCacheManagement();
            HttpResponse<List<GetUserResponse>> response = await _serviceProvider.GetCacheManagement();
            List<GetUserResponse> cacheUsers = response.Body;

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.IsEmpty(cacheUsers);
        }
    }
}
