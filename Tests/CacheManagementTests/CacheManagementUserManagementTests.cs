﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.Models.Responses;
using UserServiceAPITests.ServiceProvider;
using UserServiceAPITests.UserManagementTests;

namespace UserServiceAPITests.Tests.CacheManagementTests
{
    public class CacheManagementUserManagementTests
    {
        private UserServiceServiceProvider _serviceProvider = new UserServiceServiceProvider();
        private GenerateUsersRequest _generateUsersRequest = new GenerateUsersRequest();
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetCacheManagement_CreateUser_ResponseStatusIsOk([Values(0, 1, 7, 15)] int numberOfUsers)
        {

            //Precondition            
            List<CreateUserRequest> requestUsers = _generateUsersRequest.generateUsers(numberOfUsers);
            List<int> usersCreated = new List<int>();
            List<SetUserStatusModel> expectedUserStatus = new List<SetUserStatusModel>();
                //Create users
            foreach (CreateUserRequest request in requestUsers)
            {
                HttpResponse<int> response = await _serviceProvider.CreateUser(request);
                usersCreated.Add(response.Body);
            }

                //Delete current cache
            await _serviceProvider.DeleteCacheManagement();
                //Update User status
            foreach (int userId in usersCreated)
            {
                SetUserStatusModel setUserStatus = new SetUserStatusModel
                {
                    UserId = userId,
                    NewStatus = false
                };
                
                expectedUserStatus.Add(setUserStatus);
                await _serviceProvider.SetUserStatus(setUserStatus);

            }

            //Action
            HttpResponse<List<GetUserResponse>> reponse = await _serviceProvider.GetCacheManagement();
            List<GetUserResponse> cacheUsers = reponse.Body;

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, reponse.HttpStatusCode);
            //cacheUsers.Should().BeEquivalentTo(expectedUserStatus);//Body
            for(int i = 0; i < cacheUsers.Count; i++)
            {
                var expectedUser = expectedUserStatus[i];
                var resultUSer = cacheUsers[i];
                var userCreated = requestUsers[i];

                Assert.AreEqual(expectedUser.UserId, resultUSer.id);
                Assert.AreEqual(expectedUser.NewStatus, resultUSer.isActive);
                Assert.AreEqual(userCreated.firstName, resultUSer.firstName);
                Assert.AreEqual(userCreated.lastName, resultUSer.lastName);
            }
        }

        [Test]
        public async Task DeleteCacheManagement_CreateUser_ResponseStatusIsOk([Values(0, 1, 7, 15)] int numberOfUsers)
        {

            //Precondition            
            List<CreateUserRequest> requestUsers = _generateUsersRequest.generateUsers(numberOfUsers);
            List<int> usersCreated = new List<int>();
            List<SetUserStatusModel> expectedUserStatus = new List<SetUserStatusModel>();
            //Create users
            foreach (CreateUserRequest request in requestUsers)
            {
                HttpResponse<int> response = await _serviceProvider.CreateUser(request);
                usersCreated.Add(response.Body);
            }
            
            //Update User status
            foreach (int userId in usersCreated)
            {
                SetUserStatusModel setUserStatus = new SetUserStatusModel
                {
                    UserId = userId,
                    NewStatus = false
                };

                expectedUserStatus.Add(setUserStatus);
                await _serviceProvider.SetUserStatus(setUserStatus);

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
