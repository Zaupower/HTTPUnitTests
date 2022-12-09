﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.RegisterTests
{
    public class DeleteUserTests
    {
        private UserServiceServiceProvider _serviceProvider = new UserServiceServiceProvider();
        private GenerateUsersRequest _generateUsersRequest = new GenerateUsersRequest();
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ValidUser_DeleteUser_ResponseStatusIsOk([Values(0, 1, 7, 15)] int numberOfUsers)
        {
            //Precondition
            List<CreateUserRequest> requestUsers = _generateUsersRequest.generateUsers(numberOfUsers);
            List<int> userIdResponse = new List<int>();

            foreach (CreateUserRequest requestUser in requestUsers)
            {
                HttpResponse<int> createUserResponse = await _serviceProvider.CreateUser(requestUser);
                userIdResponse.Add(createUserResponse.Body);

            }
            //Action
            foreach (int userId in userIdResponse)
            {
                var deleteResponse = await _serviceProvider.DeleteUser(userId);

                //Assert
                Assert.AreEqual(HttpStatusCode.OK, deleteResponse.HttpStatusCode);
                Assert.AreEqual(null, deleteResponse.Body);
                Assert.AreEqual("", deleteResponse.Content);
            }
        }

        [Test]
        public async Task ValidUser_DeleteUserTwoTimes_ResponseStatusIsStatusInternalServerError()
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "fisrt_name_test1",
                lastName = "last_name_test1"
            };

            HttpResponse<int> createUserResponse = await _serviceProvider.CreateUser(request);
            int userId = createUserResponse.Body;

            //Action
                //Delete first time
            await _serviceProvider.DeleteUser(userId);
                //Delete second time
            var deleteResponse = await _serviceProvider.DeleteUser(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, deleteResponse.HttpStatusCode);
            Assert.AreEqual(deleteResponse.Content, "Sequence contains no elements");
            Assert.AreEqual(null, deleteResponse.Body);
        }

        [Test]
        public async Task InvalidUser_DeleteUser_ResponseStatusIsInternalServerError()
        {
            //Precondiction
            int userId = 0;

            //Action
            var deleteResponse = await _serviceProvider.DeleteUser(userId);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, deleteResponse.HttpStatusCode);
            Assert.AreEqual(deleteResponse.Content, "Sequence contains no elements");
            Assert.AreEqual(null, deleteResponse.Body);
        }
    }
}