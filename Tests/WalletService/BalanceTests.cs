﻿using FluentAssertions;
using Newtonsoft.Json.Linq;
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

namespace UserServiceAPITests.Tests.WalletService
{
    
    public class BalanceTests
    {
        private WalletServiceServiceProvider _serviceProvider = new();
        
        [SetUp]
        public void setup()
        {

        }
        [TearDown]
        public void teardown()
        {

        }

        [Test]
        public async Task GetBalance_VerifiedUser_ResponseStatusIsOk([Values(3)] int userId)
        {
            //Precondition

            //Action
            var response = await _serviceProvider.GetBalance(userId);            
            
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);            
            Assert.IsTrue(response.Body is double);       
        }

        [Test]
        public async Task GetBalance_UnverifiedAndInvalidUser_ResponseStatusIsOk([Values(0,1)] int userId)
        {
            //Precondition
            double expectedBody = 0;
            //Action
            var response = await _serviceProvider.GetBalance(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(expectedBody, response.Body);
            Assert.AreEqual(response.Content, "not active user");
        }
    }
}