﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.UserManagementTests
{
    public class GetUserStatus
    {
        private UserServiceServiceProvider _serviceProvider = UserServiceServiceProvider.Instance;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ValidUser_GetDefaultUserStatus_ResponseStatusIsOk()
        {
            //Precondition
            
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrtNameTest1",
                lastName = "lastNameTest1"
            };

            HttpResponse<int> createUserresponse = await _serviceProvider.CreateUser(request);
            int userId = createUserresponse.Body;

            //Action 
            HttpResponse<object> getUserStatusResponse = await _serviceProvider.GetUserStatus(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, getUserStatusResponse.HttpStatusCode);
            Assert.AreEqual(false, getUserStatusResponse.Body);
            StringAssert.AreEqualIgnoringCase(getUserStatusResponse.Body.ToString(), getUserStatusResponse.Content);
        }

        [Test]
        public async Task ValidUser_GetUserStatus_ResponseStatusIsOk([Values(true, false)] bool newUserStatus)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrtNameTest1",
                lastName = "lastNameTest1"
            };

            HttpResponse<int> createUserresponse = await _serviceProvider.CreateUser(request);
            int userId = createUserresponse.Body;

            SetUserStatusModel setUserStatus = new SetUserStatusModel
            {
                UserId = createUserresponse.Body,
                NewStatus = newUserStatus
            };
            
            await _serviceProvider.SetUserStatus(setUserStatus);

            //Action 
            HttpResponse<object> getUserStatusResponse = await _serviceProvider.GetUserStatus(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, getUserStatusResponse.HttpStatusCode);
            Assert.AreEqual(newUserStatus, getUserStatusResponse.Body);
            StringAssert.AreEqualIgnoringCase(getUserStatusResponse.Body.ToString(), getUserStatusResponse.Content);
        }
    }
}