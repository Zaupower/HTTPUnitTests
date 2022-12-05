﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests;
using UserServiceAPITests.Models.Responses;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.Tests.CacheManagementTests
{
    public class CacheManagementRegisterUserTests
    {
        private UserServiceServiceProvider _serviceProvider = new UserServiceServiceProvider();
        private GenerateUsersRequest _generateUsersRequest= new GenerateUsersRequest();
        [SetUp]
        public void Setup()
        {
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
            HttpResponse<List<GetUserResponse>> reponse = await _serviceProvider.GetCacheManagement();
            List<GetUserResponse> cacheUsers = reponse.Body;

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, reponse.HttpStatusCode);
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
            HttpResponse<List<GetUserResponse>> reponse = await _serviceProvider.GetCacheManagement();
            List<GetUserResponse> cacheUsers = reponse.Body;

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, reponse.HttpStatusCode);
            Assert.IsEmpty(cacheUsers);
        }
    }
}
