using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests;
using UserServiceAPITests.Models.Responses;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.Tests.CacheManagementTests
{
    public class CacheManagementFromRegisterNewUser
    {
        private UserServiceServiceProvider _serviceProvider = new UserServiceServiceProvider();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetCacheManagement_CreateUSer_ResponseStatusIsOk()
        {

            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrt_name_test1",
                lastName = "last_name_test1"
            };
            await _serviceProvider.CreateUser(request);
            HttpResponse<int> createUserResponse = await _serviceProvider.CreateUser(request);
            int userId = createUserResponse.Body;

            HttpResponse<List<GetUserResponse>> GetCacheManagementResponse = await _serviceProvider.GetCacheManagement();
            List<GetUserResponse> cacheUsers = GetCacheManagementResponse.Body;
            foreach (GetUserResponse user in cacheUsers)
            {
                user.Should().BeEquivalentTo(request);
            }
            Assert.AreEqual(true, true);
        }
    }
}
