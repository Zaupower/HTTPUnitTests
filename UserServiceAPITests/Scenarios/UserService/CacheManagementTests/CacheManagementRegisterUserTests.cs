using CommonLogic.Models.Responses.Base;
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
using UserServiceAPITests.Models.Responses.UserService;
using UserServiceAPITests.Scenarios;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.Tests.CacheManagementTests
{
    [TestFixture]
    public class CacheManagementRegisterUserTests : UserServiceBaseTest
    {
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
